using Application.OrderManagement.Dtos.GroupedOrder;
using Application.OrderManagement.Dtos.SingleOrder;

namespace Application.OrderManagement.Services
{
	public interface IOrderReportService
	{
		Task<SingleOrderReportDto?> ReportSingleOrderAsync(string orderId);
		Task<GroupedOrderReportDto?> ReportGroupedOrderAsync(string orderId);
		Task<GroupedOrderTransactionReportDto?> ReportGroupedOrderTranasctionAsync(string orderId,string transactionOrderId);
	}
}
