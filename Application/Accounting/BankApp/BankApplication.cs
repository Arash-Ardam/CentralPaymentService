using Application.Abstractions;
using Application.Accounting.BankApp.Dtos;
using Application.Accounting.BankApp.Events;
using Application.Accounting.BankApp.Services;
using AutoMapper;
using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using MediatR;

namespace Application.Accounting.BankApp
{
	internal class BankApplication : IBankApplication
	{
		private readonly IBankRepository _bankRepository;
		private readonly IMediator _mediator;
		private readonly IBankQueryService _bankQueryService;

		public BankApplication(IBankRepository bankRepository,IMediator mediator, IBankQueryService bankQueryService)
		{
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_bankQueryService = bankQueryService ?? throw new ArgumentNullException(nameof(bankQueryService));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateBankDto bankDto)
		{
			var response = new ApplicationResponse<Guid>();
			try
			{
				if (await _bankQueryService.IsExists(bankDto.Name, bankDto.BankCode))
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.ValidationError;
					response.Message = $"The given Bank with name:{bankDto.Name} and code:{bankDto.BankCode} already excists";
					return response;
				}

				Bank newBank = new Bank(bankDto.Name, bankDto.BankCode);

				var createdBank = await _bankRepository.AddAsync(newBank);

				response.Data = createdBank.Id;
				response.Status = ApplicationResultStatus.Created;
				response.Message = $"Bank wiht Id:{createdBank.Id} created successfully";
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

		public async Task<ApplicationResponse> AssignPaymentServices(Guid bankId, List<ServiceTypes> services)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId);

				if (targetBank is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
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


		public async Task<ApplicationResponse> ChangeStatusAsync(Guid bankId, bool status)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				Bank targetBank = await _bankRepository.GetAsync(bankId);

				if (targetBank is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"given target bank is not excists";
					return response;
				}

				targetBank.ChangeStatus(status);

				await _bankRepository.EditAsync(targetBank);

				// raise disable related accounts event
				await _mediator.Publish(new BankStatusChangedEvent(bankId, status));

				response.Message = "Bank status changed successfully";
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

		public async Task<ApplicationResponse<BankInfoDto>> GetAsync(Guid bankId)
		{
			var response = new ApplicationResponse<BankInfoDto>() { IsSuccess = true };
			try
			{
				response.Data = await _bankQueryService.GetAsync(bankId);
				if (response.Data is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = "Bank with givenId not found";
					return response;
				}

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

		public async Task<ApplicationResponse<List<BankInfoDto>>> GetAll()
		{
			var response = new ApplicationResponse<List<BankInfoDto>>() { IsSuccess = true };
			try
			{
				response.Data = await _bankQueryService.GetAllAsync();
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
	}
}
