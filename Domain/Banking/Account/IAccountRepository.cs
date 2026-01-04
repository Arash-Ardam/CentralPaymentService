namespace Domain.Banking.Account
{
	public interface IAccountRepository
	{
		Task<Guid> AddAsync(Account account);
		Task<Guid> EditAsync(Account account);
		Task EditRangeAsync(List<Account> accounts);
		Task<Guid> DeleteAsync(Guid accountId);

		Task<Account> GetAsync(Guid accountId);

		Task<List<Account>> GetByBankId(Guid bankId);
		Task<List<Account>> GetByCustomerId(Guid customerId);
	}
}
