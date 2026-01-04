using Domain.Banking.Bank.Enums;
using Domain.Order.Enums;

namespace Domain.Banking.Bank.ValueObjects
{
	public class BankGlobalPolicy
	{
		internal BankGlobalPolicy() { }

		public int MaxDailyTransactionCount { get; set; }

		public List<ServiceTypes> ServiceTypes { get; set; }

	}
}
