using Application.Abstractions;
using Application.Abstractions.Dtos;
using Application.Abstractions.Services;
using Application.OrderManagement.Dtos.SingleOrder;
using Application.OrderManagement.Enums;
using Application.OrderManagement.Mappings;
using Application.OrderManagement.Services;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;
using Domain.Order.Enums;
using System.Text.Json;

namespace Application.OrderManagement
{
	internal sealed class SingleOrderApplication : ISingleOrderApplication
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IBankRepository _bankRepository;
		private readonly IOutboxMessageService _outboxMessageService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderReportService _reportService;
		private readonly IPaymentServicesFactory _paymentServiceFactory;

		public SingleOrderApplication(IAccountRepository accountRespository,
							 IOrderRepository orderRepository,
							 ICustomerRepository customerRepository,
							 IPaymentServicesFactory pspServiceFactory,
							 IBankRepository bankRepository,
							 IPaymentPolicyService paymentPolicyService,
							 IOutboxMessageService outboxMessageService,
							 IOrderReportService reportService,
							 IUnitOfWork unitOfWork)
		{
			_accountRepository = accountRespository ?? throw new ArgumentNullException(nameof(accountRespository));
			_orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
			_customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
			_paymentServiceFactory = pspServiceFactory ?? throw new ArgumentNullException(nameof(pspServiceFactory));
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_outboxMessageService = outboxMessageService ?? throw new ArgumentNullException(nameof(outboxMessageService));
			_reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		}

		public async Task<ApplicationResponse<Guid>> CreateAsync(CreateSingleOrderDto orderDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				var targetAccount = await _accountRepository.GetAsync(orderDto.AccountId);
				if (targetAccount is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"target account with id:{orderDto.AccountId} not found";
					return response;
				}

				var bank = await _bankRepository.GetAsync(targetAccount.BankId);
				if (bank is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"Invalid bank";
					return response;
				}

				var customer = await _customerRepository.GetAsync(targetAccount.CustomerId);
				if (customer is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"Invalid customer";
					return response;
				}

				bank.EnsureHasSingleService();
				targetAccount.EnsureSingleServiceAvailable();

				var order = OrderFactory.CreateSingle(targetAccount.Id, orderDto.Amount, orderDto.Description);
				var result = await _orderRepository.CreateAsync(order);

				await PublishEvent(customer, targetAccount, order);
				await _unitOfWork.SaveTenantChangesAsync();

				response.Data = result.Id;
				response.Status = ApplicationResultStatus.Created;
				response.Message = "Single order drafted successfully";
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

		public async Task<ApplicationResponse<Guid>> AddTransaction(SingleTransactionDto transactionDto)
		{
			var response = new ApplicationResponse<Guid>() { IsSuccess = true };
			try
			{
				var targerOrder = await _orderRepository.GetAsync(transactionDto.OrderId);

				if (targerOrder is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"Target order by id:{transactionDto.OrderId} not found";
					return response;
				}

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
				await PublishEvent(targerOrder, OrderStatus.Drafted, OutboxBehaviorType.AddSpecs);

				await _unitOfWork.SaveTenantChangesAsync();

				response.Data = targerOrder.Id;
				response.Status = ApplicationResultStatus.Accepted;
				response.Message = "transaction added successfully";
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

		public async Task<ApplicationResponse> RemoveTransaction(Guid OrderId)
		{
			var response = new ApplicationResponse() { IsSuccess = true };

			try
			{
				var targerOrder = await _orderRepository.GetAsync(OrderId);
				if (targerOrder is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"Target order by id:{OrderId} not found";
					return response;
				}

				targerOrder.RemoveSingleTransaction();
				await _orderRepository.UpdateAsync(targerOrder);
				await PublishEvent(targerOrder, OrderStatus.Drafted, OutboxBehaviorType.AddSpecs);


				await _unitOfWork.SaveTenantChangesAsync();

				response.Message = "transaction removed successfully";
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

		public async Task<ApplicationResponse> FinalizeOrder(Guid OrderId)
		{
			var response = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var targerOrder = await _orderRepository.GetAsync(OrderId);
				if (targerOrder is null)
				{
					response.IsSuccess = false;
					response.Status = ApplicationResultStatus.NotFound;
					response.Message = $"Target order by id:{OrderId} not found";
					return response;
				}

				targerOrder.FinalizeSingleOrder();

				await _orderRepository.UpdateAsync(targerOrder);
				await PublishEvent(targerOrder, OrderStatus.Submited, OutboxBehaviorType.Submit);
								
				await _unitOfWork.SaveTenantChangesAsync();

				response.Message = "order finalized and ready to proccess";
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

		//Send To Single Payment Provider Service
		public async Task<ApplicationResponse> SendOrderAsync(Guid orderId)
		{
			var applicationResponse = new ApplicationResponse() { IsSuccess = true };
			try
			{
				var loadResponse = await LoadOrderRequiredContexts(orderId);
				if (loadResponse.IsFailed)
				{
					applicationResponse.IsSuccess = false;
					applicationResponse.Status = loadResponse.Status;
					applicationResponse.Message = loadResponse.Message;
					return applicationResponse;
				}

				(Order order, Bank bank, Account account, Customer customer) = loadResponse.Data;

				order.EnsurePayable();

				var request = SingleOrderMapper.MapToRequest(order, account, customer);

				var pspService = _paymentServiceFactory.GetPSPPaymentService(bank.Code);
				if (pspService is null)
				{
					applicationResponse.IsSuccess = false;
					applicationResponse.Message = "No PSP service for bank";
					applicationResponse.Status = ApplicationResultStatus.NotFound;
					return applicationResponse;
				}

				var providerResponse = await pspService.SendRequestAsync(request);

				if (providerResponse.IsSuccess)
					order.MarkSingleRequestAsSent(providerResponse.Data.TrackingCode, providerResponse.Message);
				else
					order.MarkSingleRequestStatus(providerResponse.Data.Status, providerResponse.Message);

				await _orderRepository.UpdateAsync(order);
				await PublishEvent(order, OrderStatus.Pending, OutboxBehaviorType.SentToBank);

				await _unitOfWork.SaveTenantChangesAsync();

				applicationResponse.IsSuccess = providerResponse.IsSuccess;
				applicationResponse.Status = ApplicationResultStatus.Accepted;
				applicationResponse.Message = providerResponse.Message;
				return applicationResponse;
			}
			catch (Exception ex)
			{
				applicationResponse.IsSuccess = false;
				applicationResponse.Status = ApplicationResultStatus.Exception;
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
				var loadResponse = await LoadOrderRequiredContexts(orderId);
				if (loadResponse.IsFailed)
				{
					applicationResponse.IsSuccess = false;
					applicationResponse.Status = loadResponse.Status;
					applicationResponse.Message = loadResponse.Message;
					return applicationResponse;
				}

				(Order order, Bank bank, Account account, Customer customer) = loadResponse.Data;

				order.EnsureSent();

				var pspService = _paymentServiceFactory.GetPSPPaymentService(bank.Code);
				if (pspService is null)
				{
					applicationResponse.IsSuccess = false;
					applicationResponse.Message = "No PSP service for bank";
					applicationResponse.Status = ApplicationResultStatus.NotFound;
					return applicationResponse;
				}

				var inquiryRequest = SingleOrderMapper.MapToInquiryRequest(order.TrackingCode);

				var response = await pspService.InquiryAsync(inquiryRequest);
				if (response.IsSuccess)
				{
					order.MarkSingleRequestStatus(response.Data.Status, response.Message);
					await _orderRepository.UpdateAsync(order);
				}

				await PublishEvent(order, order.Specifics.Status, OutboxBehaviorType.Inquiry);
				await _unitOfWork.SaveTenantChangesAsync();

				applicationResponse.IsSuccess = response.IsSuccess;
				applicationResponse.Status = ApplicationResultStatus.Accepted;
				applicationResponse.Message = response.Message;

				return applicationResponse;
			}
			catch (Exception ex)
			{
				applicationResponse.IsSuccess = false;
				applicationResponse.Status = ApplicationResultStatus.Exception;
				applicationResponse.Message = ex.Message;
				return applicationResponse;
			}
		}

		public async Task<ApplicationResponse<SingleOrderReportDto>> ReportAsync(string orderId)
		{
			var response = new ApplicationResponse<SingleOrderReportDto>() { IsSuccess = true };
			try
			{
				var report = await _reportService.ReportSingleOrderAsync(orderId);

				if(report is null)
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

		private async Task PublishEvent(Order targerOrder,OrderStatus status,OutboxBehaviorType type)
		{
			var outbox = await _outboxMessageService.FindAsync(targerOrder.OrderId);
			if (outbox is not null)
			{
				await _outboxMessageService.PublishAsync(new OutboxMessageDto
				{
					OutboxId = targerOrder.OrderId,
					Type = OutBoxType.SingleOrder,
					BehaviorType = type,
					TenantId = outbox.TenantId,
					TenantName = outbox.TenantName,
					Payload = Enum.GetName(typeof(OrderStatus),status)	
				});

				await _unitOfWork.SaveAdminChangesAsync();
			}
		}

		private async Task PublishEvent(Customer customer, Account account, Order order)
		{
			var payload = JsonSerializer.Serialize(new SingleOrderReportDto
			{
				OwnerFullName = $"{customer.Info.FirstName} {customer.Info.LastName}",
				SourceAccount = account.AccountNumber,
				Amount = order.Specifics.Amount,
				Description = order.Specifics.Description,
				Status = OrderStatus.Drafted,
				TenantName = customer.TenantName,
				OrderId = order.OrderId,
			});

			await _outboxMessageService.PublishAsync(new OutboxMessageDto
			{
				OutboxId = order.OrderId,
				Type = OutBoxType.SingleOrder,
				BehaviorType = OutboxBehaviorType.Create,
				Payload = payload,
				TenantId = customer.Id,
				TenantName =customer.TenantName
			});

			await _unitOfWork.SaveAdminChangesAsync();
			
		}
		private async Task<ApplicationResponse<(Order order, Bank bank, Account account, Customer customer)>> LoadOrderRequiredContexts(Guid orderId)
		{
			var targetOrder = await _orderRepository.GetAsync(orderId);
			if (targetOrder == null)
				return ApplicationGuard.ValidationError<(Order order, Bank bank, Account account, Customer customer)>($"Target order by id:{orderId} not found");

			var account = await _accountRepository.GetAsync(targetOrder.SourceAccountId);
			if (account is null)
				return ApplicationGuard.ValidationError<(Order order, Bank bank, Account account, Customer customer)>("Invalid account");

			var customer = await _customerRepository.GetAsync(account.CustomerId);
			if (customer is null)
				return ApplicationGuard.ValidationError<(Order order, Bank bank, Account account, Customer customer)>("Invalid customer");

			var bank = await _bankRepository.GetAsync(account.BankId);
			if (bank is null)
				return ApplicationGuard.ValidationError<(Order order, Bank bank, Account account, Customer customer)>("Invalid bank");

			return new ApplicationResponse<(Order order, Bank bank, Account account, Customer customer)>()
			{
				Data = (targetOrder, bank, account, customer),
				IsSuccess = true
			};
		}

	
	}
}
