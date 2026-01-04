namespace Domain.Order.Enums
{
	public enum GroupedTransactionStatus
	{
		None = 0,
		Drafted = 1,
		Pending = 2,
		WaitForProvider = 3,
		Succeded = 4,
		Failed = 5,
		Canceled = 6,
		RolledBack = 7,
		Rejected = 8
	}
}
