using Application.Accounting.CustomerApp;
using Application.Accounting.CustomerApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CentralPaymentWebApi.Controllers.Accounting
{
	public class CustomerController : ApiControllerBase
	{
		public CustomerController(ICustomerApplication customerApplication)
		{
			_customerApp = customerApplication ?? throw new ArgumentNullException(nameof(customerApplication));
		}

		public ICustomerApplication _customerApp { get; }

		[HttpPost(RouteTemplates.Create)]
		[ProducesResponseType(statusCode:StatusCodes.Status201Created)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerDto createDto)
		{
			try
			{
				var appResponse = await _customerApp.CreateAsync(createDto);
				ActionResult = HandleOutput(appResponse);

				return ActionResult;

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("setSettings")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SetSettingsAsync(InformationDto infoDto)
		{
			try
			{
				var appResponse = await _customerApp.SetCustomerSettings(infoDto);
				ActionResult = HandleOutput(appResponse);

				return ActionResult;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{customerId}/status")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SetStatusAsync([FromRoute]Guid customerId, [FromBody]bool status)
		{
			try
			{
				var appResponse = await _customerApp.ChangeStatus(customerId,status);
				ActionResult = HandleOutput(appResponse);

				return ActionResult;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet(RouteTemplates.Get)]
		[ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(CustomerInfoDto))]
		[ProducesResponseType(statusCode:StatusCodes.Status404NotFound)]
		[ProducesResponseType(statusCode:StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAsync([FromQuery] Guid customerId)
		{
			try
			{
				var appResponse = await _customerApp.GetAsync(customerId);
				ActionResult = HandleOutput(appResponse);

				return ActionResult;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
