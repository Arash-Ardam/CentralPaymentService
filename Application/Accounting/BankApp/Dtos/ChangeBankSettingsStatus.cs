using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Dtos
{
	public record ChangeBankSettingsStatus(Guid BankId,bool Status);
}
