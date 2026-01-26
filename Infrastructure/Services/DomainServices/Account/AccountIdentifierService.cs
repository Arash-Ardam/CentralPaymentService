using Domain.Banking.Account.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.DomainServices.Account
{
	internal class AccountIdentifierService : IAccountIdentifierService
	{
		public AccountIdentifierService(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public AdminEfCoreDbContext _dbContext { get; }

		public Task<bool> IsExists(string accountNubmer, string accountIban)
		{
			return _dbContext.Accounts.AnyAsync(acc => acc.AccountNumber == accountNubmer && acc.Iban == accountIban);	
		}
	}
}
