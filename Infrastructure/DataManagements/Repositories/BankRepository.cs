using Domain.Banking.Bank;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Repositories
{
	internal class BankRepository : IBankRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public BankRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;		
		}

		public async Task<Bank> AddAsync(Bank bank)
		{
			await _dbContext.Banks.AddAsync(bank);
			await _dbContext.SaveChangesAsync();

			return bank;
		}

		public async Task<Bank> EditAsync(Bank bank)
		{
			_dbContext.Banks.Update(bank);
			await _dbContext.SaveChangesAsync();

			return bank;
		}

		public async Task<Bank> GetAsync(Guid bankId)
		{
			return await _dbContext.Banks.FirstOrDefaultAsync(bank => bank.Id == bankId);
		}

		
	}
}
