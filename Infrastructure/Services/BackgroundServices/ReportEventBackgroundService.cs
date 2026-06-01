using Application.OrderManagement.Enums;
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
				/* 
				 الان:

var adminDb = _serviceScopeFactory
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<AdminEfCoreDbContext>();

اینجا Scope ساخته شده ولی Dispose نشده.

همین مشکل برای:

var tenantDb = _serviceScopeFactory
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<TenantEfCoreDbContext>();

هم وجود داره.

بهتر:

using var scope = _serviceScopeFactory.CreateScope();

var adminDb =
    scope.ServiceProvider
        .GetRequiredService<AdminEfCoreDbContext>();

و برای Tenant هم مشابه.
				 */
				using var  scope = _serviceScopeFactory.CreateScope();
				var adminDb = scope.ServiceProvider.GetRequiredService<AdminEfCoreDbContext>();

				var tenants = await adminDb.Customers
					.AsNoTracking()
					.Select(x => new
					{
						id = x.Id,
						name = x.TenantName,
						connectionString = x.ConnectionString	
					}).ToListAsync(stoppingToken);
				

				foreach (var tenant in tenants)
				{
					var connectionString = string.IsNullOrWhiteSpace(tenant.connectionString) ? 
						string.Format(_options.EfCore.TenantConnectionString,tenant.name): tenant.connectionString;

					var options = new DbContextOptionsBuilder<TenantEfCoreDbContext>()
						.UseSqlServer(connectionString)
						.Options;

					await using var tenantDb =
						new TenantEfCoreDbContext(options);

					var orderEvents = await tenantDb.OrderEvents
						.Where(x => x.Processed == false)
						.OrderBy(x => x.CreatedAt)
						.Take(100)
						.ToListAsync(stoppingToken);

					if (!orderEvents.Any())
						continue;

					foreach (var orderEvent in orderEvents)
					{
						try
						{
							if (orderEvent.RetryCount >= 5)
							{
								orderEvent.Processed = true;
								orderEvent.Error = "Retry count hits";
								continue;
							}

							if (orderEvent.PaymentType == PaymentType.Single)
							{
								SingleOrderReportModel? reportModel;

								try
								{
									reportModel = JsonSerializer.Deserialize<SingleOrderReportModel>(
										orderEvent.Payload);
								}
								catch (JsonException ex)
								{
									orderEvent.Error = ex.ToString();
									orderEvent.RetryCount++;
									continue;
								}

								if (reportModel != null)
								{
									switch (orderEvent.EventType)
									{
										case OrderEventType.Create:
											{
												var exists =
												await tenantDb.SingleOrderReports
													.AnyAsync(
														x => x.OrderId == reportModel.OrderId,
														stoppingToken);

												if (!exists)
													await tenantDb.SingleOrderReports
														.AddAsync(reportModel, stoppingToken);
											}
											break;
										default:
											{
												var tenantReport = await tenantDb.SingleOrderReports.FirstOrDefaultAsync(report => report.OrderId == orderEvent.OrderId);
												if (tenantReport != null)
												{
													tenantReport.OrderId = reportModel.OrderId;
													tenantReport.TenantName = reportModel.TenantName;
													tenantReport.OwnerFullName = reportModel.OwnerFullName;
													tenantReport.SourceAccount = reportModel.SourceAccount;
													tenantReport.Amount = reportModel.Amount;
													tenantReport.Description = reportModel.Description;
													tenantReport.DepositFullName = reportModel.DepositFullName;
													tenantReport.DepositAccount = reportModel.DepositAccount;
													tenantReport.Status = reportModel.Status;
												}	
											}
											break;
									}								
								}
							}
							// TODO: PaymentType.Grouped flow

							// mark event as proccessed
							orderEvent.Processed = true;
							orderEvent.ProcessedAt = DateTimeOffset.UtcNow;
						}


						catch (Exception ex)
						{
							orderEvent.Processed = false;
							orderEvent.RetryCount++;
							orderEvent.Error = ex.ToString();
						}
					}

					await tenantDb.SaveChangesAsync(stoppingToken);
				}


				await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
			}
		}
	}
}
