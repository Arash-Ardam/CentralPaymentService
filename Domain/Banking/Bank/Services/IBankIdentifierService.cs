using Domain.Banking.Bank.Enums;

namespace Domain.Banking.Bank.Services
{
	public interface IBankIdentifierService
	{
		Task<bool> IsBankExists(string name, BankCode bankCode);
	}
}
