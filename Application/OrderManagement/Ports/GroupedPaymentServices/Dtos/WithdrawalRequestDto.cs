using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.GroupedPaymentServices.Dtos
{
	public record WithdrawalRequestDto
	{
		public string SourceAccountNumber { get; set; }
		public string SourceAccountIban { get; set; }
		public string SourceFirstName { get; set; } = string.Empty;
		public string SourceLastName { get; set; } = string.Empty;
		public string RequestPaymentId { get; set; } = string.Empty;

		public List<WithdrawalRequestTransactions> Transactions { get; set; } = new List<WithdrawalRequestTransactions>();


	}

	public record WithdrawalRequestTransactions
	{
		public string AccountNumber { get; set; }
		public string Iban { get; internal set; } = string.Empty;
		public string NationalId { get; internal set; } = string.Empty;
		public TransactionType TransactionType { get; internal set; } = TransactionType.None;
		public string PaymentId { get; internal set; } = string.Empty;


		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; }

	}

}
