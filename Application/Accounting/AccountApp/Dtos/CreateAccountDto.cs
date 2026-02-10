namespace Application.Accounting.AccountApp.Dtos
{
	public class CreateAccountDto
	{
		public Guid BankId { get; set; }
		public Guid CustomerId { get; set; }
		public string Accountnumber { get; set; }
		public string Iban { get; set; } = string.Empty;
		public DateTimeOffset ExpireDate { get; set; } 
	}
}
