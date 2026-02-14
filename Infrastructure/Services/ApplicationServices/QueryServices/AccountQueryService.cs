using Application.Accounting.AccountApp.Dtos;
using Application.Accounting.AccountApp.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices.QueryServices
{
	internal class AccountQueryService : IAccountQueryService
	{
		private readonly AdminEfCoreDbContext _dbContext;

		public AccountQueryService(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<AccountInfoDto>> GetAllAsync()
		{
			return await
			(
				from account in _dbContext.Accounts.AsNoTracking()
				join bank in _dbContext.Banks on account.BankId equals bank.Id
				join customer in _dbContext.Customers on account.CustomerId equals customer.Id
				select new AccountInfoDto
				{
					Id = account.Id,
					AccountNumber = account.AccountNumber,
					Iban = account.Iban,
					ExpirationDate = account.ExpirationDate.Value,
					Status = account.IsEnable,

					BankName = bank.Name,

					CustomerName =
						(customer.Info != null)
						? (customer.Info.FirstName + " " + customer.Info.LastName).Trim()
						: string.Empty,

					SingleService = account.PaymentSettings.Single == null
						? null
						: new SingleSettingsInfoDto
						{
							TerminalId = account.PaymentSettings.Single.TerminalId,
							MerchantId = account.PaymentSettings.Single.MerchantId,
							Username = account.PaymentSettings.Single.Username,
							Password = account.PaymentSettings.Single.Password,
							ExpireDate = account.PaymentSettings.Single.ContractExpire,
							Status = account.PaymentSettings.Single.IsEnable
						},

					BatchService = account.PaymentSettings.Batch == null
						? null
						: new BatchSettingsInfoDto
						{
							MaxDailyAmount = account.PaymentSettings.Batch.MaxDailyAmount,
							MaxTransactionsCount = account.PaymentSettings.Batch.MaxTransactionsCount,
							MinSatnaAmount = account.PaymentSettings.Batch.MinSatnaAmount,
							ExpireDate = account.PaymentSettings.Batch.ContractExpire,
							Status = account.PaymentSettings.Batch.IsEnable
						}
				}
			).ToListAsync();
		}

		public async Task<AccountInfoDto?> GetAsync(Guid accountId)
		{
			return await
				(
					from account in _dbContext.Accounts.AsNoTracking()
					where account.Id == accountId
					join bank in _dbContext.Banks.AsNoTracking() on account.BankId equals bank.Id
					join customer in _dbContext.Customers.AsNoTracking() on account.CustomerId equals customer.Id
					select new AccountInfoDto
					{
						Id = account.Id,
						AccountNumber = account.AccountNumber,
						Iban = account.Iban,
						BankName = bank.Name,
						CustomerName = $"{customer.Info.FirstName} {customer.Info.LastName}".Trim(),
						ExpirationDate = account.ExpirationDate.Value,
						Status = account.IsEnable,
						SingleService = account.PaymentSettings.Single == null
						? null
						: new SingleSettingsInfoDto
						{
							TerminalId = account.PaymentSettings.Single.TerminalId,
							MerchantId = account.PaymentSettings.Single.MerchantId,
							Username = account.PaymentSettings.Single.Username,
							Password = account.PaymentSettings.Single.Password,
							ExpireDate = account.PaymentSettings.Single.ContractExpire,
							Status = account.PaymentSettings.Single.IsEnable
						},

						BatchService = account.PaymentSettings.Batch == null
						? null
						: new BatchSettingsInfoDto
						{
							MaxDailyAmount = account.PaymentSettings.Batch.MaxDailyAmount,
							MaxTransactionsCount = account.PaymentSettings.Batch.MaxTransactionsCount,
							MinSatnaAmount = account.PaymentSettings.Batch.MinSatnaAmount,
							ExpireDate = account.PaymentSettings.Batch.ContractExpire,
							Status = account.PaymentSettings.Batch.IsEnable
						}
					}
				).FirstOrDefaultAsync();
		}

		public Task<bool> IsExists(string accountNubmer, string accountIban)
		{
			return _dbContext.Accounts.AsNoTracking().AnyAsync(acc => acc.AccountNumber == accountNubmer && acc.Iban == accountIban);
		}
	}
}
