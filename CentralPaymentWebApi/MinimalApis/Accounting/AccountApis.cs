using Application.Abstractions;
using Application.Accounting.AccountApp;
using Application.Accounting.AccountApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Configurations.EndpointsFilter;
using CentralPaymentWebApi.Dtos.AccountApi;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Accounting
{
	public static class AccountApis
	{
		public static RouteGroupBuilder MapAccountApis(this RouteGroupBuilder group)
		{
			group
				 .MapPost(RouteTemplates.Create, async Task<ApiResponse<Guid>> (IAccountApplication accountApp, [FromBody] CreateAccountDto dto) =>
				 {
					 try
					 {
						 var appResponse = await accountApp.CreateAsync(dto);
						 return appResponse.HandleApiResponse();
					 }
					 catch (Exception ex)
					 {
						 return new ApiResponse<Guid>
						 {
							 HttpResult = Results.InternalServerError(ex.Message),
							 AppResponse = new ApplicationResponse<Guid>
							 {
								 IsSuccess = false,
								 Message = ex.Message,
								 Status = ApplicationResultStatus.Exception
							 }
						 };
					 }
				 })
				 .RequireIdempotency<Guid>()
				 .WithDisplayName("CreateAccount")
				 .WithSummary("ایجاد حساب پرداخت جدید")
				 .WithDescription("ایجاد حساب پرداخت جدید در سیستم")
				 .Produces(statusCode: StatusCodes.Status201Created)
				 .Produces<string>(statusCode: StatusCodes.Status400BadRequest)
				 .Produces<string>(statusCode: StatusCodes.Status500InternalServerError);

			group
				.MapPut("changeStatus", async Task<ApiResponse<Guid>> (IAccountApplication accountApp, [FromBody] ChangeAccountStatusDto dto) =>
				{
					try
					{
						var appResponse = await accountApp.ChangeStatusAsync(dto.AccountId,dto.Status);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<Guid>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<Guid>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.RequireIdempotency<Guid>()
				.WithDisplayName("ChangeStatus")
				.WithSummary("تغییر وضعیت حساب پرداخت")
				.WithDescription("تغییر وضعیت حساب پرداخت در سیستم")
				.Produces(statusCode: StatusCodes.Status202Accepted)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest)
				.Produces<string>(statusCode: StatusCodes.Status500InternalServerError);

			group
				.MapPost("setSinglePaymentService", async Task<ApiResponse<Guid>> (IAccountApplication accountApp, [FromBody] SingleSettingsDto dto) =>
				{
					try
					{
						var appResponse = await accountApp.AddSinglePaymentSettings(dto);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<Guid>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<Guid>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.RequireIdempotency<Guid>()
				.WithDisplayName("SetSinglePaymentService")
				.WithSummary("تنظیم سرویس پرداخت تکی برای حساب")
				.WithDescription("این متد یک سرویس پرداخت تکی برای حساب ایجاد می‌کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("singleService/changeStatus", async Task<ApiResponse> (IAccountApplication accountApp, [FromBody] ChangeAccountStatusDto dto) =>
				{
					try
					{
						var appResponse = await accountApp.ChangeSingleSettingsStatus(dto.AccountId, dto.Status);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						}; 
					}
				})
				.RequireIdempotency()
				.WithDisplayName("SetSinglePaymnetServiceStatus")
				.WithSummary("تغییر وضعیت سرویس پرداخت تکی")
				.WithDescription("این متد وضعیت سرویس پرداخت تکی در صورت وجو داشتن برای حساب را تغییر می‌دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("setGroupedPaymentService", async Task<ApiResponse<Guid>> (IAccountApplication accountApp, [FromBody] BatchSettingsDto dto) =>
				{
					try
					{
						var appResponse = await accountApp.AddBatchPaymentSettings(dto);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<Guid>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<Guid>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.RequireIdempotency<Guid>()
				.WithDisplayName("setGroupedPaymentService")
				.WithSummary("ایجاد سرویس پرداخت گروهی برای حساب")
				.WithDescription("این متد یک سرویس پرداخت گروهی برای حساب ایجاد می‌کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("groupedService/changeStatus", async Task<ApiResponse> (IAccountApplication accountApp, [FromBody] ChangeAccountStatusDto dto) =>
				{
					try
					{
						var appResponse = await accountApp.ChangeBatchSettingsStatus(dto.AccountId, dto.Status);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.RequireIdempotency()
				.WithDisplayName("SetGroupedPaymentServiceStatus")
				.WithSummary("تغییر وضعیت سرویس پرداخت گروهی")
				.WithDescription("این متد وضعیت سرویس پرداخت گروهی در صورت وجود داشتن برای حساب را تغییر می‌دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);	

			group
				.MapGet("{accountId:guid}", async Task<ApiResponse<AccountInfoDto>> (IAccountApplication accountApp, Guid accountId) =>
				{
					try
					{
						var appResponse = await accountApp.GetAsync(accountId);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<AccountInfoDto>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<AccountInfoDto>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithApiResponse<AccountInfoDto>()
				.WithDisplayName("GetAccountInfo")
				.WithSummary("دریافت اطلاعات حساب پرداخت")
				.WithDescription("این متد اطلاعات حساب پرداخت را بر اساس شناسه حساب دریافت می‌کند")
				.Produces<AccountInfoDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);	 

			group
				.MapGet("", async Task<ApiResponse<List<AccountInfoDto>>> (IAccountApplication accountApp) =>
				{
					try
					{
						var appResponse = await accountApp.GetAllAsync();
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<List<AccountInfoDto>>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<List<AccountInfoDto>>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithDisplayName("GetAllAccounts")
				.WithSummary("دریافت لیست حساب‌های پرداخت")
				.WithDescription("این متد لیست تمام حساب‌های پرداخت را دریافت می‌کند")
				.Produces<List<AccountInfoDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);	 

			return group;
		}
	}
}
