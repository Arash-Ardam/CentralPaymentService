using Application.Accounting.BankApp.Dtos;
using Application.Accounting.BankApp.Services;
using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices.QueryServices
{
	internal class BankQueryService : IBankQueryService
	{
		private readonly AdminEfCoreDbContext _dbContext;
		public BankQueryService(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}
		public Task<List<BankInfoDto>> GetAllAsync()
		{
			return (
				from bank in _dbContext.Banks.AsNoTracking()
				select new BankInfoDto
				{
					Code = bank.Code,
					Id = bank.Id,
					Title = bank.Name,
					Status = bank.isEnable,
					ServiceTypes = bank.ServiceTypes
				}
				).ToListAsync();
		}

		public Task<BankInfoDto?> GetAsync(Guid bankId)
		{
			return (
				from bank in _dbContext.Banks.AsNoTracking()
				where bank.Id == bankId
				select new BankInfoDto
				{
					Code = bank.Code,
					Id = bank.Id,
					Title = bank.Name,
					Status = bank.isEnable,
					ServiceTypes = bank.ServiceTypes
				}
				).FirstOrDefaultAsync();
		}

		public Task<bool> IsExists(string name, BankCode bankCode)
		{
			return _dbContext.Banks.AsNoTracking().AnyAsync(bank => bank.Name == name && bank.Code == bankCode);
		}
	}
}
