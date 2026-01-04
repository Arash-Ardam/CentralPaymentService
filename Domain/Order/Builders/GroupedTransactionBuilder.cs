using Domain.Order.Entities;
using Domain.Order.Enums;
using Domain.Order.ValueObjects;

namespace Domain.Order.Builders
{
	public class GroupedTransactionBuilder
	{
		internal GroupedTransactionBuilder() { }
		private Guid OrderId;
		private long Amount;
		private string Description = string.Empty;
		private string FirstName;
		private string LastName;
		private string AccountNumber = string.Empty;
		private string Iban;
		private string NationalCode;
		private int PaymentId = Random.Shared.Next(1000000000, 999999999);
		private TransactionType TransactionType;


		public GroupedTransactionBuilder ForOrder(Guid orderId)
		{
			this.OrderId = orderId;
			return this;
		}

		public GroupedTransactionBuilder WithAmount(long value)
		{
			if (value == 0)
				throw new ArgumentException("Amount should be at least one");
			this.Amount = value;
			return this;
		}

		public GroupedTransactionBuilder WithDescription(string description)
		{
			Description = description;
			return this;
		}

		public GroupedTransactionBuilder WithFirstName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("FirstName must have value");

			FirstName = value;
			return this;
		}

		public GroupedTransactionBuilder WithLastName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("LastName must have value");

			LastName = value;
			return this;
		}

		public GroupedTransactionBuilder ToAccountNumber(string value)
		{
			this.AccountNumber = value;
			return this;
		}

		public GroupedTransactionBuilder ToIban(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Iban must have value");

			Iban = value;
			return this;
		}

		public GroupedTransactionBuilder ToNationalCode(string value)
		{
			NationalCode = value;
			return this;
		}

		public GroupedTransactionBuilder WithType(TransactionType type)
		{
			TransactionType = type;
			return this;
		}

		public GroupedTransaction Build()
		{
			var transactionSpecs = new GroupedTransactionSpecs();

			transactionSpecs.Amount = this.Amount;
			transactionSpecs.Description = this.Description;
			transactionSpecs.FirstName = this.FirstName;
			transactionSpecs.LastName = this.LastName;

			transactionSpecs.AccountNumber = this.AccountNumber;
			transactionSpecs.Iban = this.Iban;
			transactionSpecs.NationalId = this.NationalCode;
			transactionSpecs.PaymentId = this.PaymentId.ToString();

			transactionSpecs.TransactionType = this.TransactionType;

			var transaction = new GroupedTransaction(OrderId, transactionSpecs);

			return transaction;
		}

	}
}
