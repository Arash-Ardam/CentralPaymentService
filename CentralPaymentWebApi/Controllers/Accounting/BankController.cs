using Application.Accounting.BankApp;
using Application.Accounting.BankApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Dtos.BankApi;
using Domain.Banking.Bank.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Accounting
{
	public class BankController : ApiControllerBase
	{
		public BankController(IBankApplication bankApplication)
		{
			_bankApplication = bankApplication ?? throw new ArgumentNullException(nameof(bankApplication));
		}

		public IBankApplication _bankApplication { get; }

		[HttpPost(RouteTemplates.Create)]
		[ProducesResponseType(statusCode: StatusCodes.Status201Created)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateBankDto createDto)
		{
			if (createDto.BankCode == null)
				return BadRequest("BankCode is null");

			var appResponse = await _bankApplication.CreateAsync(createDto);
			return HandleOutput(appResponse);
		}

		[HttpGet(RouteTemplates.Get)]
		[ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(BankInfoDto))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAsync([FromRoute] Guid id)
		{
			var appResponse = await _bankApplication.GetAsync(id);
			return HandleOutput(appResponse);
		}

		[HttpGet("getAll")]
		[ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<BankInfoDto>))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAllAsync()
		{
			var appResponse = await _bankApplication.GetAll();
			return HandleOutput(appResponse);
		}


		[HttpPost("AssignServices")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AssignServicesAsync([FromBody] AssignServiceDto assignServiceDto)
		{
			if (assignServiceDto.ServiceTypes.Contains(ServiceTypes.None))
				return BadRequest("Invalid service types");

			var appResponse = await _bankApplication.AssignPaymentServices(assignServiceDto.BankId, assignServiceDto.ServiceTypes);
			return HandleOutput(appResponse);
		}

		[HttpPost("ChangeStatus")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ChangeStatusAsync([FromBody] ChangeBankStatusDto statusDto)
		{
			var appResponse = await _bankApplication.ChangeStatusAsync(statusDto.Id,statusDto.Status);
			return HandleOutput(appResponse);
		}
	}
}
