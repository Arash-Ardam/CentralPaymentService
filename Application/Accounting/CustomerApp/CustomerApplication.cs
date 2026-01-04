using Application.Abstractions;
using Application.Accounting.CustomerApp.Dtos;
using Application.Accounting.CustomerApp.Events;
using Domain.Customer;
using Domain.Customer.Factories;
using Domain.Customer.Services;
using MediatR;

namespace Application.Accounting.CustomerApp
{
	internal class CustomerApplication
	{
		public ICustomerRepository _customerRepository { get; }
		public ICustomerService _customerService { get; }
		public IMediator _mediator { get; }

		public CustomerApplication(ICustomerRepository customerRepository, ICustomerService customerService,IMediator mediator)
		{
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateCustomerDto createCustomerDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				bool isExists = await _customerService.isCustomerExists(createCustomerDto.TenantName);

				if (isExists)
					throw new ArgumentException($"Customer with tenantName: {createCustomerDto.TenantName} already exists");

				Customer customer = new Customer(createCustomerDto.TenantName);
				Guid createdCustomerId = await _customerRepository.AddAsync(customer);

				response.Data = createdCustomerId;
				response.Message = "Customer created successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
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
					infoFactory.WithFirstName(informationDto.LastName);

				if (!string.IsNullOrWhiteSpace(informationDto.NationalCode))
					infoFactory.WithFirstName(informationDto.NationalCode);

				var customerInfo = infoFactory.Build();

				targetCustomer.SetInformation(customerInfo);

				await _customerRepository.EditAsync(targetCustomer);

				response.Data = targetCustomer.Id;
				response.Message = "Customer settings set successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> ChangeStatus(Guid customerId,bool status)
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
				response.Message = "Customer status changed successfully";
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
