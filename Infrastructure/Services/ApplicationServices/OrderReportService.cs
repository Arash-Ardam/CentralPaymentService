using Application.OrderManagement.Dtos.GroupedOrder;
using Application.OrderManagement.Dtos.SingleOrder;
using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices
{
	internal sealed class OrderReportService : IOrderReportService
	{
		private readonly TenantEfCoreDbContext _tenantDb;
		public OrderReportService(TenantEfCoreDbContext tenantDb)
		{
			_tenantDb = tenantDb ?? throw new ArgumentNullException(nameof(tenantDb));
		}

		public Task<GroupedOrderReportDto?> ReportGroupedOrderAsync(string orderId)
			=> _tenantDb.GroupedOrderReports
			.Where(report => report.OrderId == orderId)
			.Select(rep => new GroupedOrderReportDto
			{
				OwnerFullName = rep.OwnerFullName,
				Amount = rep.Amount,
				Description = rep.Description,
				NumberOfTransactions = rep.NumberOfTransactions,
				OrderId = rep.OrderId,
				SourceAccount = rep.SourceAccount,
				SourceIban = rep.SourceIban,
				Status = rep.Status,
				TenantName = rep.TenantName,
				TrackingCode = rep.TrackingCode,
				Transactions = rep.Transactions.Select(trxRep => new GroupedOrderTransactionReportDto
				{
					FullName = trxRep.FullName,
					TenantName = trxRep.TenantName,
					TrackingCode = trxRep.TrackingCode,
					Amount = trxRep.Amount,
					Description = trxRep.Description,
					Iban = trxRep.Iban,
					OrderId = trxRep.OrderId,
					Status = trxRep.Status
				}).ToList()
			}).FirstOrDefaultAsync();

		public Task<GroupedOrderTransactionReportDto?> ReportGroupedOrderTranasctionAsync(string orderId, string transactionOrderId)
			=> _tenantDb.GroupedTransactionsReports
			.Where(report => report.WithdrawalOrderId == orderId && report.OrderId == transactionOrderId)
			.Select(rep => new GroupedOrderTransactionReportDto
			{
				FullName = rep.FullName,
				TenantName = rep.TenantName,
				TrackingCode = rep.TrackingCode,
				Amount = rep.Amount,
				Description = rep.Description,
				Iban = rep.Iban,
				OrderId = rep.OrderId,
				Status = rep.Status
			}).FirstOrDefaultAsync();
		

		public Task<SingleOrderReportDto?> ReportSingleOrderAsync(string orderId)
			=> _tenantDb.SingleOrderReports
				.Where(report => report.OrderId == orderId)
				.Select(x => new SingleOrderReportDto
				{
					OwnerFullName = x.OwnerFullName,
					SourceAccount = x.SourceAccount,
					Amount = x.Amount,
					Description = x.Description,
					Status = x.Status,
					DepositFullName = x.DepositFullName,
					DepositAccount = x.DepositAccount,
					OrderId = orderId,
					TenantName = x.TenantName
				}).FirstOrDefaultAsync();
	}
}
