using Domain.Banking.Account;
using Infrastructure.DataManagements.Abstractions;

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
			throw new NotImplementedException();
		}

		public Task<Guid> DeleteAsync(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task<Guid> EditAsync(Account account)
		{
			throw new NotImplementedException();
		}

		public Task EditRangeAsync(List<Account> accounts)
		{
			throw new NotImplementedException();
		}

		public Task<Account> GetAsync(Guid accountId)
		{
			throw new NotImplementedException();
		}

		public Task<List<Account>> GetByBankId(Guid bankId)
		{
			throw new NotImplementedException();
		}

		public Task<List<Account>> GetByCustomerId(Guid customerId)
		{
			throw new NotImplementedException();
		}
	}
}
