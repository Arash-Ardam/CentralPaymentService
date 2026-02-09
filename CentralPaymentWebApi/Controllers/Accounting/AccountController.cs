using Application.Accounting.AccountApp;
using Application.Accounting.AccountApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Accounting
{
	public class AccountController : ApiControllerBase
	{
		private readonly IAccountApplication _AccountApp;
		public AccountController(IAccountApplication accountApplication)
		{
			_AccountApp = accountApplication ?? throw new ArgumentNullException(nameof(accountApplication));
		}

		[HttpPost(RouteTemplates.Create)]
		[ProducesResponseType(statusCode: StatusCodes.Status201Created)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateAccountDto createAccountDto)
		{
			try
			{
				var appResponse = await _AccountApp.CreateAsync(createAccountDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		

	}
}
