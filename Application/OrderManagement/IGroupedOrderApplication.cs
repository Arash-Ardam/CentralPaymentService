using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Application.OrderManagement.Dtos.GroupedOrder;

namespace Application.OrderManagement
{
	public interface IGroupedOrderApplication
	{
		Task<ApplicationResponse<Guid>> CreateAsync(CreateGroupedOrderDto orderDto);
		Task<ApplicationResponse> AddTransactions(AddGroupedTransactionDto addGroupedTransactionDto);
		Task<ApplicationResponse> RemoveTransaction(Guid orderId, Guid transactionId);
		Task<ApplicationResponse> RemoveRangeOfTransactions(Guid orderId, List<Guid> transactionIds); 
		Task<ApplicationResponse> FinalizeOrder(Guid orderId);
		Task<ApplicationResponse> SendOrderAsync(Guid orderId);
		Task<ApplicationResponse> InquiryPaymentOrder(Guid orderId);
		Task<ApplicationResponse> InquiryPaymentTransaction(Guid orderId, Guid transactionId);
		Task<ApplicationResponse<GroupedOrderReportDto>> ReportOrderAsync(string orderId);
		Task<ApplicationResponse<GroupedOrderTransactionReportDto>> ReportTrasnactionAsync(string orderId, string transactionOrderId);
		Task<ApplicationResponse<List<AccountInfoDto>>> GetActiveAccounts();
	}
}
