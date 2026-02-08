using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Domain.Banking.Account;
using Domain.Banking.Account.Services;
using Domain.Banking.Bank;
using Domain.Customer;
using System.Runtime.InteropServices;

namespace Application.Accounting.AccountApp
{
	internal class AccountApplication : IAccountApplication
	{
		public IAccountRepository _accountRepository { get; }
		public ICustomerRepository _customerRepository { get; }
		public IBankRepository _bankRepository { get; }
		public IAccountIdentifierService _accountIdentifierService { get; }

		public AccountApplication(
			IAccountRepository accountRepository,
			ICustomerRepository customerRepository,
			IBankRepository bankRepository,
			IAccountIdentifierService accountIdentifierService)
		{
			_accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_accountIdentifierService = accountIdentifierService ?? throw new ArgumentNullException(nameof(accountIdentifierService));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateAccountDto createAccountDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };

			try
			{
				var targetBank = await _bankRepository.GetAsync(createAccountDto.BankId)
				?? throw new ArgumentException($"given target bank with Id: {createAccountDto.BankId} not exists");

				var targetCustomer = await _customerRepository.GetAsync(createAccountDto.CustomerId)
					?? throw new ArgumentException($"given target customer with Id: {createAccountDto.CustomerId} not exists");

				var isAccountExists = await _accountIdentifierService.IsExists(createAccountDto.Accountnumber, createAccountDto.Iban);
				if (!isAccountExists)
					throw new ArgumentException($"given target account with AccountNumber:{createAccountDto.Accountnumber} and Iban: {createAccountDto.Iban} already exists");

				var newAccount = new Account(createAccountDto.Accountnumber, createAccountDto.Iban, createAccountDto.ExpireDate);

				newAccount.AssignToBank(targetBank.Id);
				newAccount.AssignToCustomer(targetCustomer.Id);

				var result = await _accountRepository.AddAsync(newAccount);

				response.Data = result;
				response.Message = "Account created successfully";

				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> ChangeStatusAsync(Guid accountId,bool status)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };

			try
			{
				var targetAccount = await _accountRepository.GetAsync(accountId)
					?? throw new ArgumentException("given target account not exists");

				targetAccount.ChangeStatus(status);

				var result = await _accountRepository.EditAsync(targetAccount);

				response.Data = result;
				response.Message = "Account status changed successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> AddSinglePaymentSettings(SingleSettingsDto settingsDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };

			try
			{
				var targetAccount = await _accountRepository.GetAsync(settingsDto.AccountId)
					?? throw new ArgumentException("given target account not exists");

				var settingFactory = AccountFactory.GetSinglePaymentSettingsFactory();

				if (!string.IsNullOrWhiteSpace(settingsDto.TerminalId))
					settingFactory.WithTerminalId(settingsDto.TerminalId);

				if (!string.IsNullOrWhiteSpace(settingsDto.MerchantId))
					settingFactory.WithMerchantId(settingsDto.MerchantId);

				if (!string.IsNullOrWhiteSpace(settingsDto.username))
					settingFactory.WithUsername(settingsDto.username);

				if (!string.IsNullOrWhiteSpace(settingsDto.password))
					settingFactory.WithPassword(settingsDto.password);

				if (settingsDto.contractExpire.HasValue)
					settingFactory.WithExpire(settingsDto.contractExpire.Value);

				var setting = settingFactory.Build();

				targetAccount.PaymentSettings.SetSingleSettings(setting);

				var result = await _accountRepository.EditAsync(targetAccount);

				response.Data = result;
				response.Message = "Single settings added successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse> ChangeSingleSettingsStatus(Guid accountId, bool status)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var targetAccount = await _accountRepository.GetAsync(accountId)
					?? throw new ArgumentException("given target account not exists");

				if (targetAccount.PaymentSettings.Single is null)
					throw new Exception("There is no single settings for this account");

				if (status)
					targetAccount.PaymentSettings.Single.Enable();
				else
					targetAccount.PaymentSettings.Single.Disable();

				response.Message = "Single settings status changed successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> AddBatchPaymentSettings(BatchSettingsDto settingsDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				var targetAccount = await _accountRepository.GetAsync(settingsDto.AccountId)
					?? throw new ArgumentException("given target account not exists");

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
				response.Message = "Batch settings added successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
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
				if (targetAccount == null)
					throw new ArgumentException("given target account not exists");

				if (targetAccount.PaymentSettings.Batch is null)
					throw new Exception("There is no batch settings for this account");

				if (status)
					targetAccount.PaymentSettings.Batch.Enable();
				else
					targetAccount.PaymentSettings.Batch.Disable();

				response.Message = "Batch settings status changed successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

	}
}
