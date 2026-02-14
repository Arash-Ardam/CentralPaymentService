namespace Application.Accounting.AccountApp.Dtos
{
	public class AccountInfoDto
	{
		public Guid Id { get; set; }
		public string AccountNumber { get; set; }
		public string Iban { get; set; }
		public DateTimeOffset ExpirationDate { get; set; }
		public bool Status { get; set; }
		public string CustomerName { get; set; }
		public string BankName { get; set; }
		public SingleSettingsInfoDto? SingleService { get; set; }
		public BatchSettingsInfoDto? BatchService { get; set; }
	}
}
