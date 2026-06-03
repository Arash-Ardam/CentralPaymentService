using Application.Abstractions;
using Application.Administration.Dtos.SingleOrder;
using Application.OrderManagement.Dtos.SingleOrder;

namespace Application.Administration
{
	public interface ISingleOrderAdminApplication
	{
		Task<ApplicationResponse<List<SingleOrderReportDto>>> FilterAsync(SingleOrderFilterDto filterDto);
	}
}
