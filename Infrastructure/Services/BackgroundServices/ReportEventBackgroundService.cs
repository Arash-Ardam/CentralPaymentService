using Application.OrderManagement.Enums;
using Domain.Order.Enums;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services.BackgroundServices
{
	internal class ReportEventBackgroundService : BackgroundService
	{
		private readonly ORMToolsOptions _options;
		private readonly ITenantRegistryService _tenantRegistryService;

		public ReportEventBackgroundService(IOptions<ORMToolsOptions> options, ITenantRegistryService tenantRegistryService)
		{
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
			_tenantRegistryService = tenantRegistryService ?? throw new ArgumentNullException(nameof(tenantRegistryService));
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var tenants = _tenantRegistryService.GetAll();

				foreach (var tenant in tenants)
				{
					var connectionString = string.IsNullOrWhiteSpace(tenant.ConnectionString) ? 
						string.Format(_options.EfCore.TenantConnectionString,tenant.Name): tenant.ConnectionString;

					var options = new DbContextOptionsBuilder<TenantEfCoreDbContext>()
						.UseSqlServer(connectionString)
						.Options;

					await using var tenantDb =
						new TenantEfCoreDbContext(options);

					if(!tenantDb.Database.CanConnect())
						continue;


					if (!tenantDb.OrderEvents.Any(x => x.Processed == false))
						continue;

					var orderEvents = await tenantDb.OrderEvents
						.Where(x => x.Processed == false)
						.OrderBy(x => x.CreatedAt)
						.Take(100)
						.ToListAsync(stoppingToken);

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
								switch (orderEvent.EventType)
								{
									case OrderEventType.Create:
										{
											try
											{
												var reportModel = JsonSerializer.Deserialize<SingleOrderReportModel>(orderEvent.Payload);
												var exists =
												await tenantDb.SingleOrderReports
													.AnyAsync(
														x => x.OrderId == orderEvent.OrderId,
														stoppingToken);

												if (!exists)
													await tenantDb.SingleOrderReports.AddAsync(reportModel, stoppingToken);
											}
											catch (Exception ex)
											{
												orderEvent.Error = ex.ToString();
												orderEvent.RetryCount++;
												continue;
											}
											break;
										}
									case OrderEventType.AddSpecs or OrderEventType.Submit or OrderEventType.SentToBank or OrderEventType.Inquiry:
										{
											var exists =
											await tenantDb.SingleOrderReports
												.AnyAsync(
													x => x.OrderId == orderEvent.OrderId,
													stoppingToken);

												var tenantReport = await tenantDb.SingleOrderReports.SingleAsync(report => report.OrderId == orderEvent.OrderId);
												tenantReport.Status = orderEvent.Status;
											break;
										}
									default:
										break;
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
