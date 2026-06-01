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
