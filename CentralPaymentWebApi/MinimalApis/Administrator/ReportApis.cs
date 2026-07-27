using Application.Abstractions;
using Application.Administration;
using Application.Administration.Dtos.SingleOrder;
using Application.OrderManagement.Dtos.SingleOrder;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Configurations.EndpointsFilter;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Administrator
{
	public static class ReportApis
	{
		public static RouteGroupBuilder MapReportApis(this RouteGroupBuilder group)
		{
			group
				.MapPost("singleOrder/report", async Task<ApiResponse<List<SingleOrderReportDto>>> (ISingleOrderAdminApplication reportApp, [FromBody] SingleOrderFilterDto dto) =>
				{
					try
					{
						var appResponse = await reportApp.FilterAsync(dto);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<List<SingleOrderReportDto>>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<List<SingleOrderReportDto>>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithApiResponse<List<SingleOrderReportDto>>()
				.WithDisplayName("FilterSingleOrderReports")
				.WithSummary("جستجوی گزارش های پرداخت تکی")
				.WithDescription("این متد گزارش های پرداخت تکی را فیلتر می کند")
				.Produces<List<SingleOrderReportDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			return group;
		}
	}
}
