using Domain.Order.Enums;

namespace Application.OrderManagement.Dtos.GroupedOrder
{
	public sealed class GroupedOrderReportDto
	{
		public string OrderId { get; set; } = string.Empty;
		public string TenantName { get; set; } = string.Empty;

		public string OwnerFullName { get; set; } = string.Empty;
		public string SourceAccount { get; set; } = string.Empty;
		public string SourceIban { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; } = string.Empty;
		public long NumberOfTransactions { get; set; }
		public OrderStatus Status { get; set; }
		public string TrackingCode { get; set; } = string.Empty;

		public List<GroupedOrderTransactionReportDto> Transactions { get; set; } = new();
	}
}
