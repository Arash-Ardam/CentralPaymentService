using Application.Abstractions;
using Application.OrderManagement.Dtos.SingleOrder;
using Application.OrderManagement.Mappings;
using Application.OrderManagement.Services;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;

namespace Application.OrderManagement
{
	public class SingleOrderApplication
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IBankRepository _bankRepository;
		private readonly IPaymentServicesFactory _paymentServiceFactory;

		public SingleOrderApplication(IAccountRepository accountRespository,
							 IOrderRepository orderRepository,
							 ICustomerRepository customerRepository,
							 IPaymentServicesFactory pspServiceFactory,
							 IBankRepository bankRepository,
							 IPaymentPolicyService paymentPolicyService)
		{
			_accountRepository = accountRespository ?? throw new ArgumentNullException(nameof(accountRespository));
			_orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_paymentServiceFactory = pspServiceFactory ?? throw new ArgumentNullException(nameof(pspServiceFactory));
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
		}


		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateSingleOrderDto orderDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };

			try
			{
				var targetAccount = await _accountRepository.GetAsync(orderDto.AccountId)
				?? throw new ArgumentException("target account not found");

				var bank = await _bankRepository.GetAsync(targetAccount.BankId)
					?? throw new ArgumentException("Invalid bank");

				bank.EnsureHasSingleService();
				targetAccount.EnsureSingleServiceAvailable();

				var order = OrderFactory.CreateSingle(targetAccount.Id, orderDto.Amount, orderDto.Description);

				var result = await _orderRepository.CreateAsync(order);

				response.Data = result.Id;
				response.Message = "Single order drafted successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse<Guid>> AddTransaction(SingleTransactionDto transactionDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				var targerOrder = await _orderRepository.GetAsync(transactionDto.OrderId)
				?? throw new ArgumentException($"Target order by id:{transactionDto.OrderId} not found");

				var transactionBuilder = OrderFactory.GetSingleTransactionBuilder();

				var transaction = transactionBuilder
					.ForOrder(transactionDto.OrderId)
					.WithAmount(transactionDto.Amount)
					.WithDescription(transactionDto.Description)
					.WithFirstName(transactionDto.FirstName)
					.WithLastName(transactionDto.LastName)
					.ToAccountNumber(transactionDto.AccountNuumber)
					.Build();

				targerOrder.AddSingleTransaction(transaction);

				await _orderRepository.UpdateAsync(targerOrder);

				response.Data = targerOrder.Id;
				response.Message = "transaction added successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}

		}

		public async Task<ApplicationResponse> RemoveTransaction(Guid OrderId)
		{
			var response = new ApplicationResponse() { IsSuccess = true };

			try
			{
				var targerOrder = await _orderRepository.GetAsync(OrderId)
					?? throw new ArgumentException($"Target order by id:{OrderId} not found");

				targerOrder.RemoveSingleTransaction();

				await _orderRepository.UpdateAsync(targerOrder);

				response.Message = "transaction removed successfully";
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		public async Task<ApplicationResponse> FinalizeOrder(Guid OrderId)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var targerOrder = await _orderRepository.GetAsync(OrderId)
				?? throw new ArgumentException($"Target order by id:{OrderId} not found");

				targerOrder.FinalizeSingleOrder();

				await _orderRepository.UpdateAsync(targerOrder);

				response.Message = "order finalized and ready to proccess";
				return response;
			}

			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
				return response;
			}
		}

		//Send To Single Payment Provider Service
		public async Task<ApplicationResponse> SendOrderAsync(Guid orderId)
		{
			var applicationResponse = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var (order, bank, account, customer) = await LoadOrderRequiredContexts(orderId);

				order.EnsurePayable();

				var request = SingleOrderMapper.MapToRequest(order, account, customer);

				var pspService = _paymentServiceFactory.GetPSPPaymentService(bank.Code)
					?? throw new ArgumentException("No PSP service for bank");

				var providerResponse = await pspService.SendRequestAsync(request);

				if (providerResponse.IsSuccess)
					order.MarkSingleRequestAsSent(providerResponse.Data.TrackingCode, providerResponse.Message);
				else
					order.MarkSingleRequestStatus(providerResponse.Data.Status, providerResponse.Message);

				await _orderRepository.UpdateAsync(order);

				applicationResponse.IsSuccess = providerResponse.IsSuccess;
				applicationResponse.Message = providerResponse.Message;
				applicationResponse.StatusCode = providerResponse.StatusCode;

				return applicationResponse;
			}
			catch (Exception ex)
			{
				applicationResponse.IsSuccess = false;
				applicationResponse.Message = ex.Message;
				return applicationResponse;
			}
		}

		// Inquiry From Single Provider Service
		public async Task<ApplicationResponse> InquiryPaymentOrder(Guid orderId)
		{
			var applicationResponse = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var (order, bank, account, customer) = await LoadOrderRequiredContexts(orderId);

				order.EnsureSent();

				var pspService = _paymentServiceFactory.GetPSPPaymentService(bank.Code)
					?? throw new ArgumentException("No PSP service for bank");

				var inquiryRequest = SingleOrderMapper.MapToInquiryRequest(order.TrackingCode);

				var response = await pspService.InquiryAsync(inquiryRequest);
				if (response.IsSuccess)
				{
					order.MarkSingleRequestStatus(response.Data.Status, response.Message);
					await _orderRepository.UpdateAsync(order);
				}

				applicationResponse.IsSuccess = response.IsSuccess;
				applicationResponse.Message = response.Message;
				applicationResponse.StatusCode = response.StatusCode;

				return applicationResponse;
			}
			catch (Exception ex)
			{
				applicationResponse.IsSuccess = false;
				applicationResponse.Message = ex.Message;
				return applicationResponse;
			}

		}


		private async Task<(Order order, Bank bank, Account account, Customer customer)> LoadOrderRequiredContexts(Guid orderId)
		{
			var targetOrder = await _orderRepository.GetAsync(orderId);
			if (targetOrder == null)
				throw new ArgumentException($"Target order by id:{orderId} not found");

			var account = await _accountRepository.GetAsync(targetOrder.SourceAccountId)
				?? throw new ArgumentException("Invalid account");

			var customer = await _customerRepository.GetAsync(account.CustomerId)
				?? throw new ArgumentException("Invalid customer");

			var bank = await _bankRepository.GetAsync(account.BankId)
				?? throw new ArgumentException("Invalid bank");

			return (targetOrder, bank, account, customer);
		}

	}
}
