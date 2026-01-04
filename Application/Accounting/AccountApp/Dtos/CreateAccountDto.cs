namespace Application.Accounting.AccountApp.Dtos
{
	public record CreateAccountDto(Guid BankId, Guid CustomerId, string Accountnumber, string Iban,DateTimeOffset ExpireDate);
}
