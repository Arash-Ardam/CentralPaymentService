using Application.OrderManagement.Dtos.SingleOrder;
using Infrastructure.DataManagements.DataModels;

namespace Infrastructure.Mappers;

internal sealed class OrderReportMapper : MapperBase
{
	public OrderReportMapper()
	{
		CreateMap<SingleOrderReportModel, SingleOrderReportDto>();
	}
}
