using Domain.Order.ValueObjects;

namespace Domain.Order.Entities
{
	public class SingleTransaction
	{
		public SingleTransaction(Guid orderId,SingleTransactionSpecs specs)
		{
			OrderId = orderId;
			Specs = specs;
		}

		public Guid Id { get; private set; } 
		public Guid OrderId { get; set; }

		public string ProviderMessage { get; internal set; } = string.Empty;

		public SingleTransactionSpecs Specs { get; private set; } = new SingleTransactionSpecs();

		
	}
}
