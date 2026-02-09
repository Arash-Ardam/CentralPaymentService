using Application.Abstractions;
using Application.Accounting.CustomerApp.Dtos;
using Application.Accounting.CustomerApp.Events;
using AutoMapper;
using Domain.Customer;
using Domain.Customer.Factories;
using Domain.Customer.Services;
using MediatR;

namespace Application.Accounting.CustomerApp
{
	internal class CustomerApplication : ICustomerApplication
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly ICustomerIdentifierService _customerIdentifierService;
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public CustomerApplication(ICustomerRepository customerRepository, ICustomerIdentifierService customerService, IMediator mediator, IMapper mapper)
		{
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_customerIdentifierService = customerService ?? throw new ArgumentNullException(nameof(customerService));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateCustomerDto createCustomerDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				bool isExists = await _customerIdentifierService.isCustomerExists(createCustomerDto.TenantName);

				if (isExists)
					throw new ArgumentException($"Customer with tenantName: {createCustomerDto.TenantName} already exists");

				Customer customer = new Customer(createCustomerDto.TenantName);
				Guid createdCustomerId = await _customerRepository.AddAsync(customer);

				response.Data = createdCustomerId;
				response.Status = ApplicarionResultStatus.Created;
				response.Message = $"Customer with Id:{createdCustomerId} created successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				response.Status = ApplicarionResultStatus.Exception;
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
				response.Status = ApplicarionResultStatus.Accepted;
				response.Message = "Customer settings set successfully";
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
				response.Status = ApplicarionResultStatus.Accepted;
				response.Message = "Customer status changed successfully";
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

		public async Task<ApplicationResponse<CustomerInfoDto>> GetAsync(Guid customerId)
		{
			var response = new ApplicationResponse<CustomerInfoDto>() { IsSuccess = true };
			try
			{
				Customer customer = await _customerRepository.GetAsync(customerId)
					?? throw new ArgumentException("Customer with given id not found");

				response.Data = _mapper.Map<CustomerInfoDto>(customer);
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

		public async Task<ApplicationResponse<List<CustomerInfoDto>>> GetAllAsync()
		{
			var response = new ApplicationResponse<List<CustomerInfoDto>>() { IsSuccess = true };
			try
			{
				List<Customer> customer = await _customerRepository.GetAllAsync();

				response.Data = _mapper.Map<List<CustomerInfoDto>>(customer);
				response.Status = ApplicarionResultStatus.Done;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				response.Status = ApplicarionResultStatus.Exception;
				return response;
			}
		}
	}
}
