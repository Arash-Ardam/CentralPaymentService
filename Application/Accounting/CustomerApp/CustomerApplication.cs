using Application.Abstractions;
using Application.Accounting.CustomerApp.Dtos;
using Application.Accounting.CustomerApp.Events;
using Application.Accounting.CustomerApp.Services;
using AutoMapper;
using Domain.Customer;
using Domain.Customer.Factories;
using MediatR;

namespace Application.Accounting.CustomerApp
{
	internal class CustomerApplication : ICustomerApplication
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly ICustomerQueryService _customerQueryService;
		private readonly IMediator _mediator;

		public CustomerApplication(ICustomerRepository customerRepository,
							 IMediator mediator,
							 ICustomerQueryService customerQueryService)
		{
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_customerQueryService = customerQueryService ?? throw new ArgumentNullException(nameof(customerQueryService));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateCustomerDto createCustomerDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				if (await _customerQueryService.IsExists(createCustomerDto.TenantName))
					throw new ArgumentException($"Customer with tenantName: {createCustomerDto.TenantName} already exists");

				Customer customer = string.IsNullOrWhiteSpace(createCustomerDto.connectionString) 
					? new Customer(createCustomerDto.TenantName) 
					: new Customer(createCustomerDto.TenantName, createCustomerDto.connectionString);

				Guid createdCustomerId = await _customerRepository.AddAsync(customer);

				response.Data = createdCustomerId;
				response.Status = ApplicationResultStatus.Created;
				response.Message = $"Customer with Id:{createdCustomerId} created successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				response.Status = ApplicationResultStatus.Exception;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> SetCustomerSettings(InformationDto informationDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				Customer targetCustomer = await _customerRepository.GetAsync(informationDto.CustomerId)
					?? throw new ArgumentException("given target Customer is not excists");

				CustomerInfoFactory infoFactory = CustomerFactory.GetInformationFactory();

				if (!string.IsNullOrWhiteSpace(informationDto.FirstName))
					infoFactory.WithFirstName(informationDto.FirstName);

				if (!string.IsNullOrWhiteSpace(informationDto.LastName))
					infoFactory.WithLastName(informationDto.LastName);

				if (!string.IsNullOrWhiteSpace(informationDto.NationalCode))
					infoFactory.WithNationalCode(informationDto.NationalCode);

				var customerInfo = infoFactory.Build();

				targetCustomer.SetInformation(customerInfo);

				await _customerRepository.EditAsync(targetCustomer);

				response.Data = targetCustomer.Id;
				response.Status = ApplicationResultStatus.Accepted;
				response.Message = "Customer settings set successfully";
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

		public async Task<ApplicationResponse<Guid>> ChangeStatus(Guid customerId, bool status)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				Customer targetCustomer = await _customerRepository.GetAsync(customerId)
					?? throw new ArgumentException("given target Customer is not excists");

				targetCustomer.ChangeStatus(status);

				await _customerRepository.EditAsync(targetCustomer);

				// raise changed related accounts status event
				await _mediator.Publish(new CustomerStatusChangedEvent(customerId, status));

				response.Data = targetCustomer.Id;
				response.Status = ApplicationResultStatus.Accepted;
				response.Message = "Customer status changed successfully";
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

		public async Task<ApplicationResponse<CustomerInfoDto>> GetAsync(Guid customerId)
		{
			var response = new ApplicationResponse<CustomerInfoDto>() { IsSuccess = true };
			try
			{
				response.Data = await _customerQueryService.GetAsync(customerId);
				if(response.Data == null)
				{
					response.IsSuccess = false;
					response.Message = "there is no customer with given id";
					response.Status = ApplicationResultStatus.ValidationError;
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

		public async Task<ApplicationResponse<List<CustomerInfoDto>>> GetAllAsync()
		{
			var response = new ApplicationResponse<List<CustomerInfoDto>>() { IsSuccess = true };
			try
			{
				response.Data = await _customerQueryService.GetAllAsync();
				response.Status = ApplicationResultStatus.Done;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				response.Status = ApplicationResultStatus.Exception;
				return response;
			}
		}
	}
}
