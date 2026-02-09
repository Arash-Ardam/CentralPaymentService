using Domain.Banking.Bank.Enums;

namespace Domain.Banking.Bank;

public interface IBankRepository
{
	public Task<Bank> AddAsync(Bank bank);
	public Task<Bank> EditAsync(Bank bank);
	public Task<Bank> GetAsync(Guid bankId);
	public Task<List<Bank>> GetAllAsync();

}
