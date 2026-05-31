using Domain.Order.Enums;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.BackgroundServices
{
	internal class ReportEventBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ORMToolsOptions _options;

		public ReportEventBackgroundService(IServiceScopeFactory serviceScopeFactory,IOptions<ORMToolsOptions> options)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var adminDb = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<AdminEfCoreDbContext>();

				var tenants = adminDb.Customers
					.AsNoTracking()
					.Select(x => new
					{
						id = x.Id,
						name = x.TenantName,
						connectionString = x.ConnectionString	
					}).ToList();

				foreach (var tenant in tenants)
				{
					var connectionString = string.IsNullOrWhiteSpace(tenant.connectionString) ? 
						string.Format(_options.EfCore.TenantConnectionString,tenant.name): tenant.connectionString;

					var tenantDb = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<TenantEfCoreDbContext>();
					tenantDb.Database.SetConnectionString(connectionString);

					var orderEvents = tenantDb.OrderEvents
						.Where(x => x.Processed == false)
						.ToList();

					if (!orderEvents.Any())
						continue;

					foreach (var orderEvent in orderEvents)
					{
						try
						{
							if (orderEvent.PaymentType == PaymentType.Single)
							{
								var reportModel = JsonSerializer.Deserialize<SingleOrderReportModel>(orderEvent.Payload);
								if (reportModel != null)
								{
									await tenantDb.SingleOrderReports.AddAsync(new SingleOrderReportModel
										{
											OrderId = reportModel.OrderId,
											TenantName = reportModel.TenantName,
											OwnerFullName = reportModel.OwnerFullName,
											SourceAccount = reportModel.SourceAccount,
											Amount = reportModel.Amount,
											Description = reportModel.Description,
											Status = reportModel.Status,
											DepositFullName = reportModel.DepositFullName,
											DepositAccount = reportModel.DepositAccount
										}, stoppingToken);

									await adminDb.SingleOrderReports.AddAsync(new SingleOrderReportModel
										{
											OrderId = reportModel.OrderId,
											TenantName = reportModel.TenantName,
											OwnerFullName = reportModel.OwnerFullName,
											SourceAccount = reportModel.SourceAccount,
											Amount = reportModel.Amount,
											Description = reportModel.Description,
											Status = reportModel.Status,
											DepositFullName = reportModel.DepositFullName,
											DepositAccount = reportModel.DepositAccount
										}, stoppingToken);
								}
							}
							// TODO: PaymentType.Grouped flow

							// mark event as proccessed
							orderEvent.Processed = true;
							orderEvent.ProcessedAt = DateTime.Now;
							tenantDb.OrderEvents.Update(orderEvent);

							await tenantDb.SaveChangesAsync(stoppingToken);
							await adminDb.SaveChangesAsync(stoppingToken);
						}
						catch (Exception ex)
						{
							orderEvent.Processed = false;
							orderEvent.Error = ex.Message;
							tenantDb.OrderEvents.Update(orderEvent);
						}
					}
				}


				await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
			}
		}
	}
}
