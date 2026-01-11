using Application.Abstractions;
using Application.Accounting.BankApp;
using Application.Accounting.BankApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using Domain.Banking.Bank.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Accounting
{
	[Route("[controller]")]
	[ApiController]
	public class BankController : ControllerBase
	{
		public BankController(IBankApplication bankApplication)
		{
			_bankApplication = bankApplication ?? throw new ArgumentNullException(nameof(bankApplication));
		}

		public IBankApplication _bankApplication { get; }

		[HttpPost(RouteTemplates.Create)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateBankDto createDto)
		{
			if(createDto.BankCode == null)
				return BadRequest("BankCode is null");

			var appResponse = await _bankApplication.CreateAsync(createDto);

			if (appResponse.IsFailed)
				return BadRequest(appResponse.Message);

			return Created(string.Empty, $"Bank with id: {appResponse.Data} created");
		}

		[HttpPost("AssignServices")]
		public async Task<IActionResult> AssignServicesAsync([FromRoute]Guid bankId,[FromBody]List<ServiceTypes> serviceTypes)
		{
			if (serviceTypes.Contains(ServiceTypes.None))
				return BadRequest("Invalid service types");

			var appResponse = await _bankApplication.AssignPaymentServices(bankId, serviceTypes);

			if(appResponse.IsFailed)
				return BadRequest(appResponse.Message);

			return Accepted(appResponse.Message);
		}


		[HttpPost("ChangeStatus")]
		public async Task<IActionResult> ChangeStatusAsync([FromRoute] Guid bankId, [FromRoute] bool status)
		{
			var appResponse = await _bankApplication.ChangeStatusAsync(bankId, status);

			if (appResponse.IsFailed)
				return BadRequest(appResponse.Message);

			return Accepted(appResponse.Message);
		}




	}
}
