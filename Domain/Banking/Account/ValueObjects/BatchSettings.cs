namespace Domain.Banking.Account.ValueObjects
{
	public class BatchSettings
	{
		public bool IsEnable { get; set; } 

		public int MaxTransactionsCount { get; private set; } 
		public long MaxDailyAmount { get; private set; }
		public long MinSatnaAmount { get; private set; }
		public DateTimeOffset ContractExpire {  get; private set; }


		internal BatchSettings(bool isEnable, int maxTransactionCount,long maxDailyAmount, long minSatnaAmount, DateTimeOffset contractExpire)
		{
			IsEnable = isEnable;
			MaxTransactionsCount = maxTransactionCount;
			MaxDailyAmount = maxDailyAmount;
			ContractExpire = contractExpire;
			MinSatnaAmount = minSatnaAmount;
		}

		public void Enable() => IsEnable = true;
		public void Disable() => IsEnable = false;

	}
}
