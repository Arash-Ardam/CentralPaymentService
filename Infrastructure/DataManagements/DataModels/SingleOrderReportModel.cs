using Domain.Order.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class SingleOrderReportModel
	{
		public int Id { get; set; }
		public string OrderId { get; set; }
		public string TenantName { get; set; }

		public string OwnerFullName { get; set; }
		public string SourceAccount { get; set; }

		public long Amount { get; set; }
		public string Description { get; set; }
		public OrderStatus Status { get; set; }

		public string DepositFullName { get; set; }
		public string DepositAccount { get; set; }
	}
}
