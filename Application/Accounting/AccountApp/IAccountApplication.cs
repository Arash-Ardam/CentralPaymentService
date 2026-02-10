using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Domain.Banking.Account;

namespace Application.Accounting.AccountApp
{
	public interface IAccountApplication
	{
		Task<ApplicationResponse<Guid>> CreateAsync(CreateAccountDto createAccountDto);
		Task<ApplicationResponse<Guid>> ChangeStatusAsync(Guid accountId, bool status);
		Task<ApplicationResponse<Guid>> AddSinglePaymentSettings(SingleSettingsDto settingsDto);
		Task<ApplicationResponse> ChangeSingleSettingsStatus(Guid accountId, bool status);
		Task<ApplicationResponse<Guid>> AddBatchPaymentSettings(BatchSettingsDto settingsDto);
		Task<ApplicationResponse> ChangeBatchSettingsStatus(Guid accountId, bool status);
		Task<ApplicationResponse<AccountInfoDto>> GetAsync(Guid accountId);
		Task<ApplicationResponse<List<AccountInfoDto>>> GetAllAsync();

	}
}
