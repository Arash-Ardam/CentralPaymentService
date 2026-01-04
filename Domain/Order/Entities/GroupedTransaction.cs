using Domain.Order.Enums;
using Domain.Order.ValueObjects;

namespace Domain.Order.Entities
{
	public class GroupedTransaction
	{
		public GroupedTransaction(Guid orderId, GroupedTransactionSpecs specs)
		{
			OrderId = orderId;
			Specs = specs;
		}

		public Guid Id { get; private set; }
		public Guid OrderId { get; set; }
		public string TrackingId { get; internal set; } = string.Empty;
		public string ProviderMessage { get; internal set; } = string.Empty;
		public GroupedTransactionStatus Status { get; internal set; } = GroupedTransactionStatus.None;

		public GroupedTransactionSpecs Specs { get; private set; } = new GroupedTransactionSpecs();
	}
}
