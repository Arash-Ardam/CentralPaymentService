using Domain.Order.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class GroupedOrderTransactionReportModel
	{
		public int Id { get; set; }
		public int WithdrawalId { get; set; }
		public string OrderId { get; set; } = string.Empty;
		public string WithdrawalOrderId { get; set; } = string.Empty;
		public string TenantName { get; set; } = string.Empty;
		public string TrackingCode { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; } = string.Empty;
		public GroupedTransactionStatus Status { get; set; }

		public string FullName { get; set; } = string.Empty;
		public string Iban { get; set; } = string.Empty;
	}
}
