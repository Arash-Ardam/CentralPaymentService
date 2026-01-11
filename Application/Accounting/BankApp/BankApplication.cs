using Application.Abstractions;
using Application.Accounting.BankApp.Dtos;
using Application.Accounting.BankApp.Events;
using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.Services;
using MediatR;

namespace Application.Accounting.BankApp
{
	internal class BankApplication : IBankApplication
	{
		private readonly IBankRepository _bankRepository;
		private readonly IBankIdentifierService _bankIdentifierService;
		private readonly IMediator _mediator;

		public BankApplication(IBankRepository bankRepository, IBankIdentifierService bankIdentifierService, IMediator mediator)
		{
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_bankIdentifierService = bankIdentifierService ?? throw new ArgumentNullException(nameof(bankIdentifierService));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateBankDto bankDto)
		{
			var response = new ApplicationResponse<Guid>();
			try
			{
				bool isExist = await _bankIdentifierService.IsBankExists(bankDto.Name, bankDto.BankCode);

				if (!isExist)
					throw new ArgumentException($"The given Bank with name:{bankDto.Name} and code:{bankDto.BankCode} already excists");

				Bank newBank = new Bank(bankDto.Name, bankDto.BankCode);

				var createdBank = await _bankRepository.AddAsync(newBank);

				response.Data = createdBank.Id;
				response.Message = "Bank created successfully";
				return response;

			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse> AssignPaymentServices(Guid bankId,List<ServiceTypes> services)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId)
					?? throw new ArgumentException("given target bank is not excists");

				targetBank.RemoveServices();

				foreach (var service in services)
				{
					targetBank.AddService(service);
				}

				await _bankRepository.EditAsync(targetBank);

				response.Message = "Payment services added to bank successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

	
		public async Task<ApplicationResponse> ChangeStatusAsync(Guid bankId, bool status)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId)
					?? throw new ArgumentException("given target bank is not excists");

				targetBank.ChangeStatus(status);

				await _bankRepository.EditAsync(targetBank);

				// raise disable related accounts event
				await _mediator.Publish(new BankStatusChangedEvent(bankId, status));

				response.Message = "Bank status changed successfully";
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
