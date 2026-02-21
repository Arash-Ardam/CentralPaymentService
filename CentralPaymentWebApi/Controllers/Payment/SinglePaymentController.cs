using Application.OrderManagement;
using Application.OrderManagement.Dtos.SingleOrder;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Payment;

[Authorize(Policy = AuthorizationConsts.UserPolicy)]
public class SinglePaymentController : ApiControllerBase
{
	private readonly ISingleOrderApplication _singleOrderApp;

	public SinglePaymentController(ISingleOrderApplication singleOrderApplication)
	{
		_singleOrderApp = singleOrderApplication;
	}


	[HttpPost(RouteTemplates.Create)]
	public async Task<IActionResult> CreateAsync(CreateSingleOrderDto createDto)
	{
		try
		{
			var appResponse = await _singleOrderApp.CreateAsync(createDto);
			return HandleOutput(appResponse);	
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}
