using Application.Administration;
using Application.Administration.Dtos.SingleOrder;
using Application.OrderManagement.Dtos.SingleOrder;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Administrator
{
	[Authorize(Policy = AuthorizationConsts.AdminPolicy)]
	public sealed class ReportController : ApiControllerBase
	{
		public readonly ISingleOrderAdminApplication _singleOrderAdminApp;
		public ReportController(ISingleOrderAdminApplication singleOrderAdminApp)
		{
			_singleOrderAdminApp = singleOrderAdminApp ?? throw new ArgumentNullException(nameof(singleOrderAdminApp));
		}


		/// <summary>
		/// جستجو پرداخت های تکی بر اساس شناسه مشتری
		/// </summary>
		/// <param name="filterDto"></param>
		/// <returns></returns>
		[HttpPost("singleOrder/report")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SingleOrderReportDto>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
		public async Task<IActionResult> FilterSingleOrderAsync(SingleOrderFilterDto filterDto)
		{
			try
			{
				var appResponse = await _singleOrderAdminApp.FilterAsync(filterDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
