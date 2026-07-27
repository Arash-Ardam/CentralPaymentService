using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Application.Accounting.BankApp;
using Application.Accounting.BankApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Configurations.EndpointsFilter;
using CentralPaymentWebApi.Dtos.BankApi;
using Domain.Banking.Bank.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Accounting
{
	public static class BankApis
	{
		public static RouteGroupBuilder MapBankApis(this RouteGroupBuilder group)
		{
			group
				 .MapPost(RouteTemplates.Create, async Task<ApiResponse<Guid>> (IBankApplication bankApp, [FromBody] CreateBankDto dto) =>
				 {
					 try
					 {
						 if (dto.BankCode == null)
							 return new ApiResponse<Guid>
							 {
								 HttpResult = Results.InternalServerError("BankCode is null"),
								 AppResponse = new ApplicationResponse<Guid>
								 {
									 IsSuccess = false,
									 Message = "BankCode is null",
									 Status = ApplicationResultStatus.Exception
								 }
							 };

						 var appResponse = await bankApp.CreateAsync(dto);
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
				 .WithDisplayName("CreateBank")
				 .WithSummary("ایجاد بانک جدید")
				 .WithDescription("ایجاد بانک جدید در سیستم")
				 .Produces(statusCode: StatusCodes.Status201Created)
				 .Produces<string>(statusCode: StatusCodes.Status400BadRequest);


			group
				.MapGet(RouteTemplates.Get, async Task<ApiResponse<BankInfoDto>> (IBankApplication bankApp, Guid id) =>
				{
					try
					{
						var appResponse = await bankApp.GetAsync(id);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<BankInfoDto>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<BankInfoDto>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithApiResponse<BankInfoDto>()
				.WithDisplayName("GetInfo")
				.WithSummary("دریافت اطلاعات بانک")
				.WithDescription("این متد اطلاعات بانک مورد نظر را بر اساس شناسه ارسالی خروجی می دهد")
				.Produces<BankInfoDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet(RouteTemplates.GetAll, async Task<ApiResponse<List<BankInfoDto>>> (IBankApplication bankApp) =>
				{
					try
					{
						var appResponse = await bankApp.GetAll();
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<List<BankInfoDto>>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<List<BankInfoDto>>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithApiResponse<List<BankInfoDto>>()
				.WithDisplayName("GetAllInfo")
				.WithSummary("دریافت اطلاعات تمامی بانک ها")
				.WithDescription("این متد اطلاعات تمامی بانک های موجود در سیستم را خروجی می دهد")
				.Produces<List<BankInfoDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("assignServices", async Task<ApiResponse> (IBankApplication bankApp, [FromBody] AssignServiceDto dto) =>
				{
					try
					{
						if (dto.ServiceTypes.Contains(ServiceTypes.None))
							return new ApiResponse
							{
								HttpResult = Results.InternalServerError("invalid service type"),
								AppResponse = new ApplicationResponse
								{
									IsSuccess = false,
									Message = "invalid service type",
									Status = ApplicationResultStatus.Exception
								}
							};

						var appResponse = await bankApp.AssignPaymentServices(dto.BankId, dto.ServiceTypes);
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
				.WithApiResponse()
				.WithDisplayName("AssignServices")
				.WithSummary("اختصاص سرویس های پرداخت به بانک")
				.WithDescription("این متد سرویس های پرداخت را به بانک مورد نظر اختصاص می دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("changeStatus", async Task<ApiResponse> (IBankApplication bankApp, [FromBody] ChangeBankStatusDto dto) =>
				{
					try
					{
						var appResponse = await bankApp.ChangeStatusAsync(dto.Id, dto.Status);
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
				.WithDisplayName("ChangeStatus")
				.WithSummary("تغییر وضعیت بانک")
				.WithDescription("این متد وضعیت بانک مورد نظر را تغییر می دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			return group;
		}
	}
}
