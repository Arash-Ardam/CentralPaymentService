using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Dtos
{
	public record CreateBankDto(string Name, BankCode BankCode);

}
