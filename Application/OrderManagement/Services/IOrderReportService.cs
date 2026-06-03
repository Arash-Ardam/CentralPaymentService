using Application.OrderManagement.Dtos.SingleOrder;

namespace Application.OrderManagement.Services
{
	public interface IOrderReportService
	{
		Task<SingleOrderReportDto?> ReportSingleOrderAsync(string orderId);
	}
}
