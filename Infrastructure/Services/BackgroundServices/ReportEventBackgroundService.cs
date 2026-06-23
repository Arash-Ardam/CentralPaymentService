using Application.OrderManagement.Enums;
using Domain.Order;
using Domain.Order.Enums;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Abstractions;
using System.Text.Json;

namespace Infrastructure.Services.BackgroundServices
{
	internal class ReportEventBackgroundService : BackgroundService
	{
		private readonly ORMToolsOptions _options;
		private readonly ITenantRegistryService _tenantRegistryService;
		private Dictionary<string, GroupedOrderReportModel> _groupedOrderReports = new();
		private Dictionary<string, SingleOrderReportModel> _singleOrderReports = new();
		private Dictionary<string, Order> _orders = new();

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
						string.Format(_options.EfCore.TenantConnectionString, tenant.Name) : tenant.ConnectionString;

					var options = new DbContextOptionsBuilder<TenantEfCoreDbContext>()
						.UseSqlServer(connectionString)
						.Options;

					await using var tenantDb =
						new TenantEfCoreDbContext(options);

					if (!tenantDb.OrderEvents.Any(x => x.Processed == false))
						continue;

					var orderEvents = await tenantDb.OrderEvents
						.Where(x => x.Processed == false)
						.OrderBy(x => x.CreatedAt)
						.Take(100)
						.ToListAsync(stoppingToken);

					var orderIds =
						orderEvents.Select(x => x.OrderId)
						.Distinct()
						.ToList();

					_groupedOrderReports =
						await tenantDb.GroupedOrderReports
							.Include(rep => rep.Transactions)
							.Where(x => orderIds.Contains(x.OrderId))
							.ToDictionaryAsync(x => x.OrderId, stoppingToken);

					_singleOrderReports =
						await tenantDb.SingleOrderReports
							.Where(x => orderIds.Contains(x.OrderId))
							.ToDictionaryAsync(x => x.OrderId, stoppingToken);

					_orders =
						await tenantDb.Orders
							.Include(x => x.GroupedTransactions)
							.Include(x => x.SingleTransaction)
							.Where(x => orderIds.Contains(x.OrderId))
							.ToDictionaryAsync(x => x.OrderId, stoppingToken);

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
								_singleOrderReports.TryGetValue(orderEvent.OrderId, out var singleReport);
								await HandleSingleOrderEvents(orderEvent, tenantDb, singleReport, stoppingToken);
							}

							if (orderEvent.PaymentType == PaymentType.Grouped)
							{
								_groupedOrderReports.TryGetValue(orderEvent.OrderId, out var groupedReport);
								_orders.TryGetValue(orderEvent.OrderId, out var order);
								await HandleGroupedOrderEvents(orderEvent, tenantDb, order, groupedReport, stoppingToken);
							}

							// mark event as proccessed
							orderEvent.Processed = true;
							orderEvent.ProcessedAt = DateTimeOffset.UtcNow;
						}


						catch (Exception ex)
						{
							orderEvent.Processed = false;
							orderEvent.RetryCount++;
							orderEvent.Error = $"{ex.Message} : {ex.InnerException?.Message}".Trim();
						}
					}

					await tenantDb.SaveChangesAsync(stoppingToken);
				}


				await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
			}
		}


		private async Task HandleSingleOrderEvents(OrderEventModel orderEvent, TenantEfCoreDbContext tenantDb, SingleOrderReportModel report, CancellationToken stoppingToken)
		{
			switch (orderEvent.EventType)
			{
				case OrderEventType.Create:
					{
						var reportModel = JsonSerializer.Deserialize<SingleOrderReportModel>(orderEvent.Payload);
						if (reportModel is null)
							throw new InvalidOperationException(
								"Payload deserialization failed");

						if (report is null)
						{
							await tenantDb.SingleOrderReports.AddAsync(reportModel, stoppingToken);
							_singleOrderReports.Add(reportModel.OrderId, reportModel);
						}
						break;
					}
				case OrderEventType.AddSpecs or OrderEventType.Submit or OrderEventType.SentToBank or OrderEventType.Inquiry:
					{
						if (report is not null)
							report.Status = orderEvent.Status;
						else
							throw new ArgumentException("order id is invalid");
						break;
					}
				default:
					break;
			}
		}


		private async Task HandleGroupedOrderEvents(OrderEventModel orderEvent, TenantEfCoreDbContext tenantDb, Order order, GroupedOrderReportModel report, CancellationToken stoppingToken)
		{
			switch (orderEvent.EventType)
			{
				case OrderEventType.Create:
					var reportModel = JsonSerializer.Deserialize<GroupedOrderReportModel>(orderEvent.Payload);
					if (reportModel is null)
						throw new InvalidOperationException(
							"Payload deserialization failed");

					if (report is null)
					{
						await tenantDb.GroupedOrderReports.AddAsync(reportModel, stoppingToken);
						_groupedOrderReports.Add(reportModel.OrderId, reportModel);
					}
					break;

				case OrderEventType.Submit:
					if (report is not null)
					{
						report.Status = orderEvent.Status;
						foreach (var transaction in report.Transactions)
						{
							transaction.Status = GroupedTransactionStatus.Pending;
						}
					}
					break;

				case OrderEventType.SentToBank or OrderEventType.Inquiry:
					if (report is not null)
					{
						var trxMap = order.GroupedTransactions.ToDictionary(x => x.Specs.PaymentId);
						foreach (var item in report.Transactions)
						{
							if (trxMap.TryGetValue(item.OrderId, out var trx))
								item.Status = trx.Status;
						}
					}
						
					break;

				case OrderEventType.AddTransactions:
					if (report is not null)
					{
						var existingIds =
						report.Transactions
							.Select(x => x.OrderId)
							.ToHashSet();

						report.Transactions.AddRange(order.GroupedTransactions
							.Where(x => !existingIds.Contains(x.Specs.PaymentId))
							.Select(trx => 
							new GroupedOrderTransactionReportModel
							{
								OrderId = trx.Specs.PaymentId,
								TenantName = report.TenantName,
								Status = trx.Status,
								Iban = trx.Specs.Iban,
								Description = trx.Specs.Description,
								Amount = trx.Specs.Amount,
								FullName = $"{trx.Specs.FirstName} {trx.Specs.LastName}".Trim(),
								TrackingCode = trx.TrackingId,
								WithdrawalOrderId = report.OrderId
							})
							.ToList());
					}
					else
						throw new ArgumentException("order id is invalid");
					break;
				case OrderEventType.RemoveTransaction:
					if(report is not null && order is not null)
						report.Transactions.Remove(report.Transactions.Single(trx => trx.OrderId == orderEvent.Payload));
					break;
				default:
					break;
			}
		}
	}
}

