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


				await using var adminDb =
					new AdminEfCoreDbContext(
						new DbContextOptionsBuilder<AdminEfCoreDbContext>()
						.UseSqlServer(_options.EfCore.BaseConnectionString)
						.Options);

				if (!adminDb.OutboxMessages.Any(x => x.Processed == false))
				{
					await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
					continue;
				}

				var outboxMessages = await adminDb.OutboxMessages
					.Where(x =>
						!x.Processed &&
						x.Type != OutBoxType.Customer &&
						x.RetryCount < 5)
					.OrderBy(x => x.CreatedAt)
					.Take(500)
					.ToListAsync(stoppingToken);

				foreach (var tenantGroup in outboxMessages.GroupBy(x => x.TenantId))
				{
					await ProcessTenantMessages(
						tenantGroup.Key,
						tenantGroup.ToList(),
						stoppingToken);
				}

				await adminDb.SaveChangesAsync(stoppingToken);
				await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
			}
		}

		private async Task ProcessTenantMessages(Guid tenantId, List<OutboxMessageModel> messages, CancellationToken token)
		{
			var tenant = _tenantRegistryService.Find(tenantId);

			if (tenant is null)
				return;

			var connectionString =
				string.IsNullOrWhiteSpace(tenant.ConnectionString)
					? string.Format(_options.EfCore.TenantConnectionString, tenant.Name)
					: tenant.ConnectionString;

			await using var tenantDb =
				new TenantEfCoreDbContext(
					new DbContextOptionsBuilder<TenantEfCoreDbContext>()
						.UseSqlServer(connectionString)
						.Options);

			var orderIds = messages
				.Select(x => x.OutboxId)
				.Distinct()
				.ToList();

			await LoadCaches(
				tenantDb,
				orderIds,
				token);

			foreach (var message in messages)
			{
				try
				{
					switch (message.Type)
					{
						case OutBoxType.SingleOrder:
							await HandleSingleAsync(message, tenantDb, token);
							break;

						case OutBoxType.GroupedOrder:
							await HandleGroupedAsync(message, tenantDb, token);
							break;
					}

					message.Processed = true;
					message.ProcessedAt = DateTimeOffset.UtcNow;
				}
				catch (Exception ex)
				{
					message.RetryCount++;

					if (message.RetryCount >= 5)
						message.Processed = true;

					message.Error = ex.ToString();
				}
			}

			await tenantDb.SaveChangesAsync(token);
		}

		private async Task LoadCaches(TenantEfCoreDbContext tenantDb, List<string> orderIds, CancellationToken token)
		{
			_orders =
				await tenantDb.Orders
					.Include(x => x.GroupedTransactions)
					.Include(x => x.SingleTransaction)
					.Where(x => orderIds.Contains(x.OrderId))
					.ToDictionaryAsync(x => x.OrderId, token);

			_groupedOrderReports =
				await tenantDb.GroupedOrderReports
					.Include(x => x.Transactions)
					.Where(x => orderIds.Contains(x.OrderId))
					.ToDictionaryAsync(x => x.OrderId, token);

			_singleOrderReports =
				await tenantDb.SingleOrderReports
					.Where(x => orderIds.Contains(x.OrderId))
					.ToDictionaryAsync(x => x.OrderId, token);
		}

		private async Task HandleSingleAsync(OutboxMessageModel outboxMessage, TenantEfCoreDbContext tenantDb, CancellationToken cancellationToken)
		{
			_singleOrderReports.TryGetValue(
					outboxMessage.OutboxId,
					out var report);

			switch (outboxMessage.BehaviorType)
			{
				case OutboxBehaviorType.Create:
					{
						var reportModel = JsonSerializer.Deserialize<SingleOrderReportModel>(outboxMessage.Payload);
						if (reportModel is null)
							throw new InvalidOperationException(
								"Payload deserialization failed");

						if (report is null)
						{
							await tenantDb.SingleOrderReports.AddAsync(reportModel, cancellationToken);
						}
						break;
					}
				case OutboxBehaviorType.AddSpecs:
					{
						if(report is not null)
						{
							_orders.TryGetValue(outboxMessage.OutboxId, out var order);
							report.DepositFullName = $"{order.SingleTransaction.Specs.FirstName} {order.SingleTransaction.Specs.LastName}".Trim();
							report.DepositAccount = order.SingleTransaction.Specs.AccountNumber;
							report.Status = Enum.Parse<OrderStatus>(outboxMessage.Payload);
						}
						else
							throw new ArgumentException("order id is invalid");
						break;
					}
				case OutboxBehaviorType.Submit or OutboxBehaviorType.SentToBank or OutboxBehaviorType.Inquiry:
					{
						if (report is not null)
							report.Status = Enum.Parse<OrderStatus>(outboxMessage.Payload);
						else
							throw new ArgumentException("order id is invalid");
						break;
					}
				default:
					break;
			}
		}

		private async Task HandleGroupedAsync(OutboxMessageModel outboxMessage, TenantEfCoreDbContext tenantDb, CancellationToken cancellationToken)
		{
			_groupedOrderReports.TryGetValue(
				outboxMessage.OutboxId,
				out var report);

			_orders.TryGetValue(
				outboxMessage.OutboxId,
				out var order);

			switch (outboxMessage.BehaviorType)
			{
				case OutboxBehaviorType.Create:
					var reportModel = JsonSerializer.Deserialize<GroupedOrderReportModel>(outboxMessage.Payload);
					if (reportModel is null)
						throw new InvalidOperationException(
							"Payload deserialization failed");

					if (report is null)
					{
						await tenantDb.GroupedOrderReports.AddAsync(reportModel, cancellationToken);
					}
					break;
				case OutboxBehaviorType.Update:
					break;
				case OutboxBehaviorType.Delete:
					break;
				case OutboxBehaviorType.DeleteAll:
					break;
				case OutboxBehaviorType.AddSpecs:
					break;
				case OutboxBehaviorType.Submit:
					if (report is not null)
					{
						report.Status = Enum.Parse<OrderStatus>(outboxMessage.Payload);
						foreach (var transaction in report.Transactions)
						{
							transaction.Status = GroupedTransactionStatus.Pending;
						}
					}
					break;
				case OutboxBehaviorType.SentToBank or OutboxBehaviorType.Inquiry:
					if (report is not null)
					{
						var trxMap = order.GroupedTransactions.ToDictionary(x => x.Specs.PaymentId);
						foreach (var item in report.Transactions)
						{
							if (trxMap.TryGetValue(item.OrderId, out var tran))
								item.Status = tran.Status;
						}
					}
					break;
				case OutboxBehaviorType.AddTransactions:
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
				case OutboxBehaviorType.RemoveTransaction:
					var trx = report.Transactions.FirstOrDefault(x => x.OrderId == outboxMessage.Payload);

					if (trx != null)
					{
						report.Transactions.Remove(trx);
					}
					break;
				default:
					break;
			}
		}
	}
}

