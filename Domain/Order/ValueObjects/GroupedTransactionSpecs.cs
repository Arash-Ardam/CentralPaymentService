using Domain.Order.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Order.ValueObjects
{
	public class GroupedTransactionSpecs
	{
		internal GroupedTransactionSpecs() { }

		public long Amount { get; internal set; } = 0;
		public string Description { get; internal set; } = string.Empty;
		public string FirstName { get; internal set; } = string.Empty;
		public string LastName { get; internal set; } = string.Empty;
		public string AccountNumber { get; internal set; } = string.Empty;
		public string Iban { get; internal set; } = string.Empty;
		public string NationalId { get; internal set; } = string.Empty;
		public string PaymentId { get; internal set; } = string.Empty;
		public TransactionType TransactionType { get; internal set; } = TransactionType.None;

	}
}
