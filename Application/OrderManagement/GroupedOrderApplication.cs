using Application.Abstractions;
using Application.Abstractions.Dtos;
using Application.Abstractions.Services;
using Application.OrderManagement.Dtos.GroupedOrder;
using Application.OrderManagement.Enums;
using Application.OrderManagement.Mappings;
using Application.OrderManagement.Services;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;
using Domain.Order.Enums;
using System.Text.Json;

namespace Application.OrderManagement;

internal class GroupedOrderApplication : IGroupedOrderApplication
{
	private readonly IAccountRepository _accountRepository;
	private readonly IOrderRepository _orderRepository;
	private readonly ICustomerRepository _customerRepository;
	private readonly IBankRepository _bankRepository;
	private readonly IPaymentServicesFactory _paymentServiceFactory;
	private readonly IPaymentPolicyService _paymentPolicyService;
	private readonly IOrderReportService _reportService;
	private readonly IOutboxMessageService _outboxMessageService;
	private readonly IUnitOfWork _unitOfWork;
	public GroupedOrderApplication(IAccountRepository accountRespository,
						 IOrderRepository orderRepository,
						 ICustomerRepository customerRepository,
						 IPaymentServicesFactory pspServiceFactory,
						 IBankRepository bankRepository,
						 IPaymentPolicyService paymentPolicyService,
						 IUnitOfWork unitOfWork,
						 IOrderReportService reportService,
						 IOutboxMessageService outboxMessageService)
	{
		_accountRepository = accountRespository ?? throw new ArgumentNullException(nameof(accountRespository));
		_orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
		_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
		_paymentServiceFactory = pspServiceFactory ?? throw new ArgumentNullException(nameof(pspServiceFactory));
		_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
		_paymentPolicyService = paymentPolicyService ?? throw new ArgumentNullException(nameof(paymentPolicyService));
		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		_reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
		_outboxMessageService = outboxMessageService ?? throw new ArgumentNullException(nameof(outboxMessageService));
	}

	public async Task<ApplicationResponse<Guid>> CreateAsync(CreateGroupedOrderDto orderDto)
	{
		var response = new ApplicationResponse<Guid>() { IsSuccess = true };

		try
		{
			var targetAccount = await _accountRepository.GetAsync(orderDto.AccountId);
			if (targetAccount is null)
			{
				response.Message = "invalid account";
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				return response;
			}

			var bank = await _bankRepository.GetAsync(targetAccount.BankId);
			if (bank is null)
			{
				response.Message = "invalid bank";
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				return response;
			}

			var customer = await _customerRepository.GetAsync(targetAccount.CustomerId);
			if (customer is null)
			{
				response.Message = "invalid customer";
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.ValidationError;
				return response;
			}

			bank.EnsureHasGroupedService();
			targetAccount.EnsureGroupedServiceAvailable();
			_paymentPolicyService.ValidateGroupPaymentRequest(targetAccount, orderDto.NumberOfTransactions, orderDto.TotalAmount);

			var order = OrderFactory.CreateGroup(orderDto.AccountId, orderDto.TotalAmount, orderDto.Description, orderDto.NumberOfTransactions);

			var result = await _orderRepository.CreateAsync(order);

			await PublishEvent(customer, targetAccount, order);

			await _unitOfWork.SaveTenantChangesAsync();

			response.Message = "Grouped order drafted successfully";
			response.Status = ApplicationResultStatus.Created;
			response.Data = result.Id;
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

	public async Task<ApplicationResponse> AddTransactions(AddGroupedTransactionDto addGroupedTransactionDto)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(addGroupedTransactionDto.OrderId);
			if (targetOrder is null)
			{
				response.IsSuccess = false;
				response.Message = $"Target order by id:{addGroupedTransactionDto.OrderId} not found";
				response.Status = ApplicationResultStatus.NotFound;
			}

			var sourceAccount = await _accountRepository.GetAsync(targetOrder.SourceAccountId);
			if (sourceAccount is null)
			{
				response.IsSuccess = false;
				response.Message = "Target order did't assinged to a valid account";
				response.Status = ApplicationResultStatus.ValidationError;
			}


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
			await PublishEvent(targetOrder, targetOrder.Specifics.Status, OutboxBehaviorType.AddTransactions);
			await _unitOfWork.SaveTenantChangesAsync();

			response.Message = "Transactions added successfully";
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

	public async Task<ApplicationResponse> RemoveTransaction(Guid orderId, Guid transactionId)
	{
		var response = new ApplicationResponse() { IsSuccess = true };
		try
		{
			var targetOrder = await _orderRepository.GetAsync(orderId);
			if (targetOrder is null)
			{
				response.IsSuccess = false;
				response.Message = $"Target order by id:{orderId} not found";
				response.Status = ApplicationResultStatus.NotFound;
			}

			var paymentId = targetOrder.GetGroupedTrasaction(transactionId).Specs.PaymentId;

			targetOrder.RemoveGroupedTransaction(transactionId);

			await _orderRepository.UpdateAsync(targetOrder);
			await PublishEvent(targetOrder, targetOrder.Specifics.Status, OutboxBehaviorType.RemoveTransaction, paymentId);

			await _unitOfWork.SaveTenantChangesAsync();

			response.Message = "Transaction removed successfully";
			response.Status = ApplicationResultStatus.Accepted;
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
			var targetOrder = await _orderRepository.GetAsync(orderId);
			if (targetOrder is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.NotFound;
				response.Message = $"Target order by id:{orderId} not found";
				return response;
			}

			targetOrder.FinalizeGroupedOrder();

			await _orderRepository.UpdateAsync(targetOrder);
			await PublishEvent(targetOrder, targetOrder.Specifics.Status, OutboxBehaviorType.Submit);

			await _unitOfWork.SaveTenantChangesAsync();
			response.Message = "Order finalized and ready to proccess";
			response.Status = ApplicationResultStatus.Accepted;
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

			await PublishEvent(order, order.Specifics.Status, OutboxBehaviorType.SentToBank);
			await _orderRepository.UpdateAsync(order);

			await _unitOfWork.SaveTenantChangesAsync();

			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			applicationResponse.Status = ApplicationResultStatus.Accepted;
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

				await PublishEvent(order, order.Specifics.Status, OutboxBehaviorType.Inquiry);
				await _orderRepository.UpdateAsync(order);

				await _unitOfWork.SaveTenantChangesAsync();
			}

			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			applicationResponse.Status = ApplicationResultStatus.Accepted;
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

			applicationResponse.Message = response.Message;
			applicationResponse.IsSuccess = response.IsSuccess;
			applicationResponse.Status = ApplicationResultStatus.Accepted;
			return applicationResponse;

		}
		catch (Exception ex)
		{
			applicationResponse.IsSuccess = false;
			applicationResponse.Message = ex.Message;
			return applicationResponse;
		}
	}

	public async Task<ApplicationResponse<GroupedOrderReportDto>> ReportOrderAsync(string orderId)
	{
		var response = new ApplicationResponse<GroupedOrderReportDto>() { IsSuccess = true };
		try
		{
			var report = await _reportService.ReportGroupedOrderAsync(orderId);

			if (report is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.NotFound;
				response.Message = "invalid order Id";
				return response;
			}

			response.Data = report;
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

	public async Task<ApplicationResponse<GroupedOrderTransactionReportDto>> ReportTrasnactionAsync(string orderId, string transactionOrderId)
	{
		var response = new ApplicationResponse<GroupedOrderTransactionReportDto>() { IsSuccess = true };
		try
		{
			var report = await _reportService.ReportGroupedOrderTranasctionAsync(orderId, transactionOrderId);

			if (report is null)
			{
				response.IsSuccess = false;
				response.Status = ApplicationResultStatus.NotFound;
				response.Message = "invalid order Id";
				return response;
			}

			response.Data = report;
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

	private async Task PublishEvent(Order targerOrder, OrderStatus status, OutboxBehaviorType behavior, string? payload = null)
	{
		var oldEvent = await _outboxMessageService.FindAsync(targerOrder.OrderId);
		if (oldEvent is not null)
		{
			await _outboxMessageService.PublishAsync(new OutboxMessageDto
			{
				OutboxId = targerOrder.OrderId,
				Type = OutBoxType.GroupedOrder,
				BehaviorType = behavior,
				TenantId = oldEvent.TenantId,
				TenantName = oldEvent.TenantName,
				Payload = string.IsNullOrWhiteSpace(payload) ? Enum.GetName(status) : payload
			});

			await _unitOfWork.SaveAdminChangesAsync();

		}
	}

	private Task PublishEvent(Customer customer, Account account, Order order)
	{
		_outboxMessageService.PublishAsync(new OutboxMessageDto
		{
			OutboxId = order.OrderId,
			Type = OutBoxType.GroupedOrder,
			BehaviorType = OutboxBehaviorType.Create,
			TenantId = customer.Id,
			TenantName = customer.TenantName,
			Payload = JsonSerializer.Serialize(new GroupedOrderReportDto
			{
				OwnerFullName = $"{customer.Info.FirstName} {customer.Info.LastName}",
				SourceAccount = account.AccountNumber,
				SourceIban = account.Iban,
				NumberOfTransactions = order.Specifics.NumberOfTransactions,
				Amount = order.Specifics.Amount,
				Description = order.Specifics.Description,
				Status = OrderStatus.Drafted,
				TenantName = customer.TenantName,
				OrderId = order.OrderId,
			})
		});

		return _unitOfWork.SaveAdminChangesAsync();
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
