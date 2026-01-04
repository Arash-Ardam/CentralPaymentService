using Domain.Order.Enums;

namespace Domain.Order.ValueObjects
{
	public class SingleTransactionSpecs
	{
		internal SingleTransactionSpecs()
		{
			
		}

		public long Amount { get; internal set; } = 0;
		public string Description { get; internal set; } = string.Empty;
		public string FirstName { get; internal set; } = string.Empty;
		public string LastName { get; internal set; } = string.Empty;
		public string AccountNumber { get; internal set; } = string.Empty;
		public string PaymentId { get; internal set;  } = string.Empty;
		public TransactionType TransactionType { get; internal set; } = TransactionType.PSP;
	}
}
