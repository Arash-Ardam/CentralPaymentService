using Domain.Banking.Bank;
using Infrastructure.DataManagements.Abstractions;

namespace Infrastructure.DataManagements.Repositories
{
	internal class BankRepository : IBankRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public BankRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;		
		}

		public Task<Bank> AddAsync(Bank bank)
		{
			throw new NotImplementedException();
		}

		public Task<Bank> EditAsync(Bank bank)
		{
			throw new NotImplementedException();
		}

		public Task<Bank> GetAsync(Guid bankId)
		{
			throw new NotImplementedException();
		}
	}
}
