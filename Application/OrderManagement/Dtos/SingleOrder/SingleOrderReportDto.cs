using Domain.Order.Enums;

namespace Application.OrderManagement.Dtos.SingleOrder
{
	public sealed class SingleOrderReportDto
	{
		public string OrderId { get; set; } = string.Empty;
		public string TenantName { get; set; } = string.Empty;

		public string OwnerFullName { get; set; } = string.Empty;
		public string SourceAccount { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; } = string.Empty;
		public OrderStatus Status { get; set; }

		public string DepositFullName { get; set; } = string.Empty;
		public string DepositAccount { get; set; } = string.Empty;
	}
}
