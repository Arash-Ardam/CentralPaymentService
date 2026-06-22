using Domain.Order.Enums;

namespace Application.OrderManagement.Dtos.GroupedOrder
{
	public sealed class GroupedOrderTransactionReportDto
	{
		public string OrderId { get; set; } = string.Empty;
		public string TenantName { get; set; } = string.Empty;
		public string TrackingCode { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; } = string.Empty;
		public GroupedTransactionStatus Status { get; set; }

		public string FullName { get; set; } = string.Empty;
		public string Iban { get; set; } = string.Empty;
	}
}
