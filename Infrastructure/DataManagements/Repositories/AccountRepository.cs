using Domain.Banking.Account;
using Domain.Banking.Bank;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Repositories
{
	internal class AccountRepository : IAccountRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public AccountRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;		
		}

		public Task<Guid> AddAsync(Account account)
		{
			_dbContext.Accounts.AddAsync(account);
			_dbContext.SaveChangesAsync();

			return Task.FromResult(account.Id);
		}

		public Task<Guid> DeleteAsync(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public async Task<Guid> EditAsync(Account account)
		{
			_dbContext.Accounts.Update(account);
			await _dbContext.SaveChangesAsync();

			return account.Id;
		}

		public Task EditRangeAsync(List<Account> accounts)
		{
			throw new NotImplementedException();
		}

		public async Task<Account> GetAsync(Guid accountId)
		{
			return await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.Id == accountId);
		}

		public async Task<List<Account>> GetByBankId(Guid bankId)
		{
			return await _dbContext.Accounts.Where(acc => acc.BankId == bankId).ToListAsync();
		}

		public async Task<List<Account>> GetByCustomerId(Guid customerId)
		{
			return await _dbContext.Accounts.Where(acc => acc.CustomerId == customerId).ToListAsync();
		}
	}
}