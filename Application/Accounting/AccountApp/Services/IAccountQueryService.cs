using Application.Accounting.AccountApp.Dtos;

namespace Application.Accounting.AccountApp.Services
{
	public interface IAccountQueryService
	{
		Task<AccountInfoDto?> GetAsync(Guid accountId);

		Task<List<AccountInfoDto>> GetAllAsync();

		Task<bool> IsExists(string accountNubmer, string accountIban);

	}
}
