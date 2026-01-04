using Domain.Banking.Account.ValueObjects;

namespace Domain.Banking.Account.Factories
{
	public class BatchPaymentSettingsFactory
	{
		private int _maxTrasactionsCount = 10000;
		private long _maxDailyAmount = 1000000000;
		private long _minSatnaAmount = 50000000000;
		private DateTimeOffset _contractExpire = DateTimeOffset.Now.AddMonths(6);

		internal BatchPaymentSettingsFactory()
		{
			
		}

		public BatchPaymentSettingsFactory WithMaxTransactions(int value)
		{
			_maxDailyAmount = value;
			return this;
		}

		public BatchPaymentSettingsFactory WithMaxDailyAmount(long value)
		{
			_maxDailyAmount = value;
			return this;
		}

		public BatchPaymentSettingsFactory WithExpirationDate(DateTimeOffset value)
		{
			_contractExpire = value;
			return this;
		}

		public BatchPaymentSettingsFactory WithMinSatnaAmount(long value)
		{
			_minSatnaAmount = value;
			return this;
		}

		public BatchSettings Build()
		{
			return new BatchSettings(
				isEnable: true,
				_maxTrasactionsCount,
				_maxDailyAmount,
				_minSatnaAmount,
				_contractExpire
				);
		}
	}
}
