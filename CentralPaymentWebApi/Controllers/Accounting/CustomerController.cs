using Application.Accounting.CustomerApp;
using Application.Accounting.CustomerApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Dtos.CustomerApi;
using Microsoft.AspNetCore.Mvc;

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
		[ProducesResponseType(statusCode: StatusCodes.Status201Created)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerDto createDto)
		{
			try
			{
				var appResponse = await _customerApp.CreateAsync(createDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("setSettings")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SetSettingsAsync([FromBody] InformationDto infoDto)
		{
			try
			{
				var appResponse = await _customerApp.SetCustomerSettings(infoDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("changeStatus")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SetStatusAsync([FromBody] ChangeCustomerStatusDto statusDto)
		{
			try
			{
				var appResponse = await _customerApp.ChangeStatus(statusDto.CustomerId, statusDto.Status);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet(RouteTemplates.Get)]
		[ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(CustomerInfoDto))]
		[ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAsync([FromRoute] Guid id)
		{
			try
			{
				var appResponse = await _customerApp.GetAsync(id);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet(RouteTemplates.GetAll)]
		[ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<CustomerInfoDto>))]
		[ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAllAsync()
		{
			try
			{
				var appResponse = await _customerApp.GetAllAsync();
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
