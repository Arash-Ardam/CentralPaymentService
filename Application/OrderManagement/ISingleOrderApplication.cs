using Application.Abstractions;
using Application.OrderManagement.Dtos.SingleOrder;

namespace Application.OrderManagement
{
	public interface ISingleOrderApplication
	{
		Task<ApplicationResponse<Guid>> CreateAsync(CreateSingleOrderDto orderDto);
		Task<ApplicationResponse<Guid>> AddTransaction(SingleTransactionDto transactionDto);
		Task<ApplicationResponse> RemoveTransaction(Guid OrderId);
		Task<ApplicationResponse> FinalizeOrder(Guid OrderId);
		Task<ApplicationResponse> SendOrderAsync(Guid orderId);
		Task<ApplicationResponse> InquiryPaymentOrder(Guid orderId);
	}
}
