using Application.Accounting.AccountApp;
using Application.Accounting.AccountApp.Dtos;
using AutoMapper;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Dtos.AccountApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Controllers.Accounting
{
	[Authorize(Policy = AuthorizationConsts.AdminPolicy)]
	public class AccountController : ApiControllerBase
	{
		private readonly IAccountApplication _AccountApp;
		private readonly IMapper _mapper;
		public AccountController(IAccountApplication accountApplication, IMapper mapper)
		{
			_AccountApp = accountApplication ?? throw new ArgumentNullException(nameof(accountApplication));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpPost(RouteTemplates.Create)]
		[ProducesResponseType(statusCode: StatusCodes.Status201Created)]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
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

		[HttpPost("changeStatus")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ChangeStatusAsync(ChangeAccountStatusDto statusDto)
		{
			try
			{
				var appResponse = await _AccountApp.ChangeStatusAsync(statusDto.AccountId, statusDto.Status);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("setSingleService")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SetSingleServiceAsync(SingleSettingsDto settingDto)
		{
			try
			{
				var appResponse = await _AccountApp.AddSinglePaymentSettings(settingDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("SingleService/setStatus")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SetSingleServiceStatusAsync(ChangeAccountStatusDto statusDto)
		{
			try
			{
				var appResponse = await _AccountApp.ChangeSingleSettingsStatus(statusDto.AccountId, statusDto.Status);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("setBatchService")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SetBatchServiceAsync(BatchSettingsDto settingDto)
		{
			try
			{
				var appResponse = await _AccountApp.AddBatchPaymentSettings(settingDto);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("BatchService/setStatus")]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(string))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SetBatchServiceStatusAsync(ChangeAccountStatusDto statusDto)
		{
			try
			{
				var appResponse = await _AccountApp.ChangeBatchSettingsStatus(statusDto.AccountId, statusDto.Status);
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet(RouteTemplates.Get)]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(AccountInfoDto))]
		[ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string), Description = "پارامتری اشتباه یا نا معتبر وارد شود رخ می دهد")]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAsync(Guid id)
		{
			try
			{
				var appResponse = await _AccountApp.GetAsync(id);
				if (appResponse.IsSuccess)
				{
					var accountInfoDto = _mapper.Map<AccountInfoDto>(appResponse.Data);
					return Ok(accountInfoDto);
				}
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet(RouteTemplates.GetAll)]
		[ProducesResponseType(statusCode: StatusCodes.Status202Accepted, Type = typeof(List<AccountInfoDto>))]
		[ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllAsync()
		{
			try
			{
				var appResponse = await _AccountApp.GetAllAsync();
				if (appResponse.IsSuccess)
				{
					var accountInfoDto = _mapper.Map<List<AccountInfoDto>>(appResponse.Data);
					return Ok(accountInfoDto);
				}
				return HandleOutput(appResponse);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
