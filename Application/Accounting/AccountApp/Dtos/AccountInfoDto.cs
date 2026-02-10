namespace Application.Accounting.AccountApp.Dtos
{
	public class AccountInfoDto
	{
		public string AccountNumber { get; private set; }
		public string Iban { get; private set; }
		public DateTimeOffset ExpirationDate { get; private set; }
		public bool Status { get; private set; }
		public string CustomerName { get; set; }
		public string BankName { get; set; }
		public SingleSettingsInfoDto? SingleService { get; set; }
		public BatchSettingsInfoDto? BatchService { get; set; }
	}
}
