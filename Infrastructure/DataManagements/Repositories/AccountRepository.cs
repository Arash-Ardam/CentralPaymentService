using Domain.Banking.Account;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Repositories
{
	internal class AccountRepository : IAccountRepository
	{
		private readonly AdminEfCoreDbContext _dbContext;

		public AccountRepository(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext;		
		}

		public async Task<Guid> AddAsync(Account account)
		{
			await _dbContext.Accounts.AddAsync(account);
			await _dbContext.SaveChangesAsync();

			return account.Id;
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

		public async Task EditRangeAsync(List<Account> accounts)
		{
			_dbContext.Accounts.UpdateRange(accounts);
			await _dbContext.SaveChangesAsync();	
		}

		public Task<List<Account>> GetAllAsync()
		{
			return _dbContext.Accounts.AsQueryable().ToListAsync();
		}

		public async Task<Account> GetAsync(Guid accountId)
		{
			return await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.Id == accountId);
		}

		public Task<List<Account>> GetByBankId(Guid bankId)
		{
			return  _dbContext.Accounts.Where(acc => acc.BankId == bankId).ToListAsync();
		}

		public Task<List<Account>> GetByCustomerId(Guid customerId)
		{
			return _dbContext.Accounts.Where(acc => acc.CustomerId == customerId).ToListAsync();
		}
	}
}