using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.DomainServices.Bank
{
	internal class BankIdentifierService : IBankIdentifierService
	{
		public BankIdentifierService(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public AdminEfCoreDbContext _dbContext { get; }

		public Task<bool> IsBankExists(string name, BankCode bankCode)
		{
			return  _dbContext.Banks.AnyAsync(bank => bank.Name == name && bank.Code == bankCode);
		}
	}
}
