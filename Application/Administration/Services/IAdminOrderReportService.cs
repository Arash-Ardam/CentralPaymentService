using Application.Administration.Dtos.SingleOrder;
using Application.OrderManagement.Dtos.SingleOrder;
using System;
using System.Collections.Generic;

namespace Application.Administration.Services
{
	public interface IAdminOrderReportService
	{
		Task<List<SingleOrderReportDto>> FilterAsync(SingleOrderFilterDto filterDto);	
	}
}
