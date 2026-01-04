using Domain.Banking.Account.Factories;

namespace Domain.Banking.Account
{
	public static class AccountFactory
	{
		public static SinglePaymentSettingsFactory GetSinglePaymentSettingsFactory() => new SinglePaymentSettingsFactory();
		public static BatchPaymentSettingsFactory GetBatchPaymentSettingsFactory() => new BatchPaymentSettingsFactory();
	}
}
