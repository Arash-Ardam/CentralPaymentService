using Application.Accounting.AccountApp.Dtos;
using System.Globalization;

namespace Application.Accounting.AccountApp.Services
{
	public interface IAccountQueryService
	{
		Task<AccountInfoDto?> GetAsync(Guid accountId);

		Task<List<AccountInfoDto>> GetAllAsync();

		Task<List<AccountInfoDto>> GetAllActiveForSinglePaymentAsync(Guid tenantId);
		Task<List<AccountInfoDto>> GetAllActiveForGroupedPaymentAsync(Guid tenantId);

		Task<bool> IsExists(string accountNubmer, string accountIban);

	}
}
