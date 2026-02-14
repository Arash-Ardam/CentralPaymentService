using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Application.Accounting.AccountApp.Services;
using AutoMapper;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;

namespace Application.Accounting.AccountApp;

internal class AccountApplication : IAccountApplication
{
	private readonly IAccountRepository _accountRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly IBankRepository _bankRepository;
	private readonly IAccountQueryService _accountQueryService;


	public AccountApplication(
		IAccountRepository accountRepository,
		ICustomerRepository customerRepository,
		IBankRepository bankRepository,
		IAccountQueryService accountQueryService)
	{
		_accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
		_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
		_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
		_accountQueryService = accountQueryService ?? throw new ArgumentNullException(nameof(accountQueryService));
	}

	public async Task<ApplicationResponse<Guid>> CreateAsync(CreateAccountDto createAccountDto)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };

		try
		{
			var targetBank = await _bankRepository.GetAsync(createAccountDto.BankId);

			var targetCustomer = await _customerRepository.GetAsync(createAccountDto.CustomerId);

			var isAccountExists = await _accountQueryService.IsExists(createAccountDto.Accountnumber, createAccountDto.Iban);

			var validation = ValidateCreateAccount(
				createAccountDto,
				targetBank,
				targetCustomer,
				isAccountExists);

			if (validation != null)
			{
				response.IsSuccess = validation.IsSuccess;
				response.Status = validation.Status;
				response.Message = validation.Message;
				return response;
			}

			var newAccount = new Account(createAccountDto.Accountnumber, createAccountDto.Iban, createAccountDto.ExpireDate);

			newAccount.AssignToBank(targetBank.Id);
			newAccount.AssignToCustomer(targetCustomer.Id);

			var result = await _accountRepository.AddAsync(newAccount);

			response.Data = result;
			response.Status = ApplicationResultStatus.Created;
			response.Message = $"Account with id: ({result}) created successfully";

			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse<Guid>> ChangeStatusAsync(Guid accountId, bool status)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };

		try
		{
			var targetAccount = await _accountRepository.GetAsync(accountId);
			if (targetAccount is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			targetAccount.ChangeStatus(status);

			var result = await _accountRepository.EditAsync(targetAccount);

			response.Data = result;
			response.Status = ApplicationResultStatus.Accepted;
			response.Message = "Account status changed successfully";
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse<Guid>> AddSinglePaymentSettings(SingleSettingsDto settingsDto)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };

		try
		{
			var targetAccount = await _accountRepository.GetAsync(settingsDto.AccountId);
			if (targetAccount is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			var settingFactory = AccountFactory.GetSinglePaymentSettingsFactory();

			if (!string.IsNullOrWhiteSpace(settingsDto.TerminalId))
				settingFactory.WithTerminalId(settingsDto.TerminalId);

			if (!string.IsNullOrWhiteSpace(settingsDto.MerchantId))
				settingFactory.WithMerchantId(settingsDto.MerchantId);

			if (!string.IsNullOrWhiteSpace(settingsDto.Username))
				settingFactory.WithUsername(settingsDto.Username);

			if (!string.IsNullOrWhiteSpace(settingsDto.Password))
				settingFactory.WithPassword(settingsDto.Password);

			if (settingsDto.ExpireDate.HasValue)
				settingFactory.WithExpire(settingsDto.ExpireDate.Value);

			var setting = settingFactory.Build();

			targetAccount.PaymentSettings.SetSingleSettings(setting);

			var result = await _accountRepository.EditAsync(targetAccount);

			response.Data = result;
			response.Status = ApplicationResultStatus.Accepted;
			response.Message = "Single settings added successfully";
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> ChangeSingleSettingsStatus(Guid accountId, bool status)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetAccount = await _accountRepository.GetAsync(accountId);
			if (targetAccount is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			if (targetAccount.PaymentSettings.Single is null)
			{

				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "There is no single settings for this account";
				return response;
			}

			if (status)
				targetAccount.PaymentSettings.Single.Enable();
			else
				targetAccount.PaymentSettings.Single.Disable();

			await _accountRepository.EditAsync(targetAccount);

			response.Message = "Single settings status changed successfully";
			response.Status = ApplicationResultStatus.Accepted;
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse<Guid>> AddBatchPaymentSettings(BatchSettingsDto settingsDto)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };
		try
		{
			var targetAccount = await _accountRepository.GetAsync(settingsDto.AccountId);
			if (targetAccount is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			var settingsFactory = AccountFactory.GetBatchPaymentSettingsFactory();

			if (settingsDto.MaxTransactionsCount > 0)
				settingsFactory.WithMaxTransactions(settingsDto.MaxTransactionsCount);

			if (settingsDto.MaxDailyAmount > 0)
				settingsFactory.WithMaxDailyAmount(settingsDto.MaxDailyAmount);

			if (settingsDto.MinSatnaAmount > 0)
				settingsFactory.WithMinSatnaAmount(settingsDto.MinSatnaAmount);

			if (settingsDto.ContractExpire.HasValue)
				settingsFactory.WithExpirationDate(settingsDto.ContractExpire.Value);

			var settings = settingsFactory.Build();

			targetAccount.PaymentSettings.SetBatchSettings(settings);

			var result = await _accountRepository.EditAsync(targetAccount);

			response.Data = result;
			response.Status = ApplicationResultStatus.Accepted;
			response.Message = "Batch settings added successfully";
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> ChangeBatchSettingsStatus(Guid accountId, bool status)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetAccount = await _accountRepository.GetAsync(accountId);
			if (targetAccount is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			if (targetAccount.PaymentSettings.Batch is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "There is no batch settings for this account";
				return response;
			}

			if (status)
				targetAccount.PaymentSettings.Batch.Enable();
			else
				targetAccount.PaymentSettings.Batch.Disable();

			await _accountRepository.EditAsync(targetAccount);

			response.Message = "Batch settings status changed successfully";
			response.Status = ApplicationResultStatus.Accepted;
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse<List<AccountInfoDto>>> GetAllAsync()
	{
		var response = new ApplicationResponse<List<AccountInfoDto>>() { IsSuccess = true };
		try
		{
			var accounts = await _accountQueryService.GetAllAsync();
			response.Data = accounts;
			response.Status = ApplicationResultStatus.Done;
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}


	public async Task<ApplicationResponse<AccountInfoDto>> GetAsync(Guid accountId)
	{
		var response = new ApplicationResponse<AccountInfoDto>() { IsSuccess = true };
		try
		{
			var targetAccount = await _accountQueryService.GetAsync(accountId);
			if (targetAccount == default)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				response.Message = "given target account not exists";
				return response;
			}

			response.Data = targetAccount;
			response.Status = ApplicationResultStatus.Done;
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Status = ApplicationResultStatus.Exception;
			response.Message = ex.Message;
			return response;
		}
	}

	private ApplicationResponse ValidateCreateAccount(
		CreateAccountDto dto,
		Bank? bank,
		Customer? customer,
		bool isAccountExists)
	{
		if (bank is null)
			return ApplicationGuard.ValidationError($"given target bank with Id: {dto.BankId} not exists");

		if (customer is null)
			return ApplicationGuard.ValidationError($"given target customer with Id: {dto.CustomerId} not exists");

		if (isAccountExists)
			return ApplicationGuard.ValidationError($"given target account with AccountNumber:{dto.Accountnumber} and Iban: {dto.Iban} already exists");

		return null;
	}

}
