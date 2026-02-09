using Application.Abstractions;
using Application.Accounting.BankApp.Dtos;
using Application.Accounting.BankApp.Events;
using AutoMapper;
using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.Services;
using MediatR;
using System.Net;

namespace Application.Accounting.BankApp
{
	internal class BankApplication : IBankApplication
	{
		private readonly IBankRepository _bankRepository;
		private readonly IBankIdentifierService _bankIdentifierService;
		private readonly IMapper _mapper;
		private readonly IMediator _mediator;

		public BankApplication(IBankRepository bankRepository, IBankIdentifierService bankIdentifierService, IMediator mediator, IMapper mapper)
		{
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_bankIdentifierService = bankIdentifierService ?? throw new ArgumentNullException(nameof(bankIdentifierService));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateBankDto bankDto)
		{
			var response = new ApplicationResponse<Guid>();
			try
			{
				bool isExist = await _bankIdentifierService.IsBankExists(bankDto.Name, bankDto.BankCode);

				if (isExist)
				{
					response.IsSuccess = false;
					response.Status = ApplicarionResultStatus.ValidationError;
					response.Message = $"The given Bank with name:{bankDto.Name} and code:{bankDto.BankCode} already excists";
					return response;
				}

				Bank newBank = new Bank(bankDto.Name, bankDto.BankCode);

				var createdBank = await _bankRepository.AddAsync(newBank);

				response.Data = createdBank.Id;
				response.Status = ApplicarionResultStatus.Created;
				response.Message = $"Bank wiht Id:{createdBank.Id} created successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Status = ApplicarionResultStatus.Exception;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse> AssignPaymentServices(Guid bankId,List<ServiceTypes> services)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId);

				if(targetBank is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicarionResultStatus.NotFound;
					response.Message = $"given target bank is not excists";
					return response;
				}

				targetBank.RemoveServices();

				foreach (var service in services)
				{
					targetBank.AddService(service);
				}

				await _bankRepository.EditAsync(targetBank);

				response.Message = "Payment services added to bank successfully";
				response.Status = ApplicarionResultStatus.Accepted;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Status = ApplicarionResultStatus.Exception;
				response.Message = ex.Message;
				return response;
			}
		}

	
		public async Task<ApplicationResponse> ChangeStatusAsync(Guid bankId, bool status)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId);

				if (targetBank is null)
				{
					response.IsSuccess = false;
					response.Status= ApplicarionResultStatus.NotFound;
					response.Message = $"given target bank is not excists";
					return response;
				}

				targetBank.ChangeStatus(status);

				await _bankRepository.EditAsync(targetBank);

				// raise disable related accounts event
				await _mediator.Publish(new BankStatusChangedEvent(bankId, status));

				response.Message = "Bank status changed successfully";
				response.Status = ApplicarionResultStatus.Accepted;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Status = ApplicarionResultStatus.Exception;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<BankInfoDto>> GetAsync(Guid bankId)
		{
			var response = new ApplicationResponse<BankInfoDto>() { IsSuccess = true };
			try
			{
				Bank bank = await _bankRepository.GetAsync(bankId);
				if(bank is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicarionResultStatus.NotFound;
					response.Message = "Bank with givenId not found";
					return response;
				}
					

				response.Data = _mapper.Map<BankInfoDto>(bank);
				response.Status = ApplicarionResultStatus.Done;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess= false;
				response.Status= ApplicarionResultStatus.Exception;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<List<BankInfoDto>>> GetAll()
		{
			var response = new ApplicationResponse<List<BankInfoDto>>() { IsSuccess = true };
			try
			{
				var banks = await _bankRepository.GetAllAsync();

				response.Data = _mapper.Map<List<BankInfoDto>>(banks);
				response.Status = ApplicarionResultStatus.Done;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Status = ApplicarionResultStatus.Exception;
				response.Message = ex.Message;
				return response;
			}
		}
	}
}
