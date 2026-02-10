namespace Application.Accounting.AccountApp.Dtos
{
	public class SingleSettingsInfoDto
	{
		public bool Status { get; set; }
		public string TerminalId { get; set; }
		public string MerchantId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public DateTimeOffset ExpireDate { get; set; }
	}
}
