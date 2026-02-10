namespace Application.Accounting.AccountApp.Dtos
{
	public class BatchSettingsInfoDto
	{
		public bool Status { get; set; }
		public int MaxTransactionsCount { get; set; }
		public long MaxDailyAmount { get; set; }
		public long MinSatnaAmount { get; set; }
		public DateTimeOffset ExpireDate { get; set; }
	}
}
