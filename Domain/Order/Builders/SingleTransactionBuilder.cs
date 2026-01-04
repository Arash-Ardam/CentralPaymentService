using Domain.Order.Entities;
using Domain.Order.ValueObjects;

namespace Domain.Order.Builders
{
	public class SingleTransactionBuilder
	{
		internal SingleTransactionBuilder() { }
		private Guid OrderId;
		private long Amount;
		private string Description = string.Empty;
		private string FirstName;
		private string LastName;
		private string AccountNumber;
		private int PaymentId = Random.Shared.Next(1000000000,999999999);


		public SingleTransactionBuilder ForOrder(Guid orderId)
		{
			this.OrderId = orderId;
			return this;
		}

		public SingleTransactionBuilder WithAmount(long value)
		{
			if (value == 0)
				throw new ArgumentException("Amount should be at least one");
			this.Amount = value;
			return this;
		}

		public SingleTransactionBuilder WithDescription(string description)
		{
			Description = description; 
			return this;
		}

		public SingleTransactionBuilder WithFirstName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("FirstName must have value");

			FirstName = value;
			return this;
		}

		public SingleTransactionBuilder WithLastName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("LastName must have value");

			LastName = value;
			return this;
		}

		public SingleTransactionBuilder ToAccountNumber(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("AccountNumber most have a valid value");

			this.AccountNumber = value;
			return this;
		}


		public SingleTransaction Build()
		{
			var transactionSpecs = new SingleTransactionSpecs();

			transactionSpecs.Amount = this.Amount;
			transactionSpecs.Description = this.Description;
			transactionSpecs.FirstName = this.FirstName;
			transactionSpecs.LastName = this.LastName;

			transactionSpecs.AccountNumber = this.AccountNumber;
			transactionSpecs.PaymentId = this.PaymentId.ToString();

			var transaction = new SingleTransaction(OrderId,transactionSpecs);

			return transaction;
		}

	}
}