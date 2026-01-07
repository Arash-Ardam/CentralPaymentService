using Application.Abstractions;
using Application.OrderManagement.Dtos.GroupedOrder;
using Application.OrderManagement.Mappings;
using Application.OrderManagement.Services;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;
using Domain.Order.Enums;

namespace Application.OrderManagement;

internal class GroupedOrderApplication
{
	private readonly IAccountRepository _accountRepository;
	private readonly IOrderRepository _orderRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly IBankRepository _bankRepository;
	private readonly IPaymentServicesFactory _paymentServiceFactory;
	private readonly IPaymentPolicyService _paymentPolicyService;

	public GroupedOrderApplication(IAccountRepository accountRespository,
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
		_paymentPolicyService = paymentPolicyService ?? throw new ArgumentNullException(nameof(paymentPolicyService));
	}

	public async Task<ApplicationResponse<Guid>> CreateAsync(CreateGroupedOrderDto orderDto)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };

		try
		{
			var targetAccount = await _accountRepository.GetAsync(orderDto.AccountId)
			?? throw new ArgumentException("target account not found");

			var bank = await _bankRepository.GetAsync(targetAccount.BankId)
				?? throw new ArgumentException("Invalid bank");

			bank.EnsureHasGroupedService();
			targetAccount.EnsureGroupedServiceAvailable();
			_paymentPolicyService.ValidateGroupPaymentRequest(targetAccount, orderDto.NumberOfTransactions, orderDto.TotalAmount);

			var order = OrderFactory.CreateGroup(orderDto.AccountId, orderDto.TotalAmount, orderDto.Description, orderDto.NumberOfTransactions);

			var result = await _orderRepository.CreateAsync(order);

			response.Message = "Grouped order drafted successfully";
			response.Data = result.Id;
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> AddTransactions(AddGroupedTransactionDto addGroupedTransactionDto)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(addGroupedTransactionDto.OrderId)
			?? throw new ArgumentException($"Target order by id:{addGroupedTransactionDto.OrderId} not found");

			var sourceAccount = await _accountRepository.GetAsync(targetOrder.SourceAccountId)
				?? throw new ArgumentException("Target order did't assinged to a valid account");

			var builder = OrderFactory.GetGroupedTransactionBuilder();

			var transactions = addGroupedTransactionDto.transactions.Select(x =>
				builder
				.ToAccountNumber(x.AccountNuumber)
				.ToIban(x.Iban)
				.ToNationalCode(x.NationalId)
				.WithAmount(x.Amount)
				.WithDescription(x.Description)
				.WithFirstName(x.FirstName)
				.WithLastName(x.LastName)
				.WithType(IdentifyTransactionType(sourceAccount, x.Amount, x.AccountNuumber))
				.Build()

			).ToList();

			targetOrder.AddGroupedTransactions(transactions);

			await _orderRepository.UpdateAsync(targetOrder);

			response.Message = "Transactions added successfully";
			return response;

		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> RemoveTransaction(Guid orderId, Guid transactionId)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(orderId)
				?? throw new ArgumentException($"Target order by id:{orderId} not found");

			targetOrder.RemoveGroupedTransaction(transactionId);

			await _orderRepository.UpdateAsync(targetOrder);

			response.Message = "Transaction removed successfully";
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> RemoveRangeOfTransactions(Guid orderId, List<Guid> transactionIds)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(orderId)
				?? throw new ArgumentException($"Target order by id:{orderId} not found");

			targetOrder.RemoveGroupedRangeTransactions(transactionIds);

			await _orderRepository.UpdateAsync(targetOrder);

			response.Message = "Transactions removed successfully";
			return response;
		}
		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
			return response;
		}
	}

	public async Task<ApplicationResponse> FinalizeOrder(Guid orderId)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(orderId)
			?? throw new ArgumentException($"Target order by id:{orderId} not found");

			targetOrder.FinalizeGroupedOrder();

			await _orderRepository.UpdateAsync(targetOrder);

			response.Message = "Order finalized and ready to proccess";
			return response;
		}

		catch (Exception ex)
		{
			response.IsSuccess = false;
			response.Message = ex.Message;
			return response;
		}
	}


	//Send to bank
	public async Task<ApplicationResponse> SendOrderAsync(Guid orderId)
	{
		var applicationResponse = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var (order, bank, account, customer) = await LoadOrderRequiredContexts(orderId);

			order.EnsurePayable();

			var provider = _paymentServiceFactory.GetWithdrawalPaymentService(bank.Code)
				?? throw new ArgumentException("there is no provider for Bank");

			var request = GroupedOrderMapper.MapToRequest(customer, account, order);

			var response = await provider.SendRequestAsync(request);

			order.MarkGroupedRequestStatus(response.Data.OrderStatus, response.Data.TrackingCode, response.Message);

			foreach (var transactionResult in response.Data.TrasactionsResponse)
			{
				var trasnaction = order.GetGroupedTrasaction(transactionResult.PaymentId);
				if (trasnaction is not null)
				{
					order.MarkGroupedRequestTransactionStatus(
					trasnaction,
					transactionResult.Status,
					transactionResult.TrackingCode,
					transactionResult.ProviderMessage);
				}

				// else => log warning

			}

			await _orderRepository.UpdateAsync(order);

			applicationResponse.StatusCode = response.StatusCode;
			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			return applicationResponse;
		}
		catch (Exception ex)
		{
			applicationResponse.IsSuccess = false;
			applicationResponse.Message = ex.Message;
			return applicationResponse;
		}
	}

	// Inquiry Grouped Payment
	public async Task<ApplicationResponse> InquiryPaymentOrder(Guid orderId)
	{
		var applicationResponse = new ApplicationResponse() { IsSuccess = true };

		try
		{
			var (order, bank, account, customer) = await LoadOrderRequiredContexts(orderId);

			order.EnsureSent();

			var withdrawalService = _paymentServiceFactory.GetWithdrawalPaymentService(bank.Code)
				?? throw new ArgumentException("No Withdrawal service for bank");

			var inquiryRequest = GroupedOrderMapper.MapToOrderInquiryRequest(order.TrackingCode);

			var response = await withdrawalService.InquiryOrderAsync(inquiryRequest);
			if (response.IsSuccess)
			{
				order.MarkGroupedRequestStatus(response.Data.status, response.Data.TrackingCode, response.Message);
				foreach (var transactionResponse in response.Data.Transactions)
				{
					var trasnaction = order.GetGroupedTrasaction(transactionResponse.PaymentId);
					if (trasnaction is not null)
					{
						order.MarkGroupedRequestTransactionStatus(
						trasnaction,
						transactionResponse.Status,
						transactionResponse.TrackingCode,
						transactionResponse.ProviderMessage);
					}
				}

				await _orderRepository.UpdateAsync(order);
			}

			applicationResponse.StatusCode = response.StatusCode;
			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			return applicationResponse;

		}
		catch (Exception ex)
		{
			applicationResponse.IsSuccess = false;
			applicationResponse.Message = ex.Message;
			return applicationResponse;
		}
	}

	public async Task<ApplicationResponse> InquiryPaymentTransaction(Guid orderId, Guid transactionId)
	{
		var applicationResponse = new ApplicationResponse() { IsSuccess = true };

		try
		{
			var (order, bank, account, customer) = await LoadOrderRequiredContexts(orderId);

			order.EnsureSent();

			var targetTransaction = order.GetGroupedTrasaction(transactionId)
				?? throw new ArgumentException($"Target transaction by id:{transactionId} not found");

			var withdrawalService = _paymentServiceFactory.GetWithdrawalPaymentService(bank.Code)
				?? throw new ArgumentException("No Withdrawal service for bank");

			var inquiryRequest = GroupedOrderMapper.MapToTransactionInquiryRequest(order.TrackingCode, targetTransaction.Specs.PaymentId);

			var response = await withdrawalService.InquiryTransactionAsync(inquiryRequest);
			if (response.IsSuccess)
			{
				order.MarkGroupedRequestTransactionStatus(
									targetTransaction,
									response.Data.Status,
									response.Data.TrackingCode,
									response.Data.ProviderMessage);

				await _orderRepository.UpdateAsync(order);
			}

			applicationResponse.StatusCode = response.StatusCode;
			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			return applicationResponse;

		}
		catch (Exception ex)
		{
			applicationResponse.IsSuccess = false;
			applicationResponse.Message = ex.Message;
			return applicationResponse;
		}
	}

	private TransactionType IdentifyTransactionType(Account sourceAccount, long Amount, string accountNumber)
	{
		var matchedSourceAccountParams = sourceAccount.AccountNumber[^4..];
		var matchedDestinationAccountParams = accountNumber[^4..];

		if (matchedSourceAccountParams == matchedDestinationAccountParams)
			return TransactionType.Internal;

		if (Amount >= sourceAccount.PaymentSettings.Batch.MinSatnaAmount)
			return TransactionType.Satna;

		return TransactionType.Paya;
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
