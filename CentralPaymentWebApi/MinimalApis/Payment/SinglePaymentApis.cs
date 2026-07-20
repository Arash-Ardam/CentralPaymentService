using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Application.OrderManagement;
using Application.OrderManagement.Dtos.SingleOrder;
using Application.OrderManagement.Services;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Configurations.EndpointsFilter;
using Infrastructure.Helpers;
using Infrastructure.Services.Idempotency;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CentralPaymentWebApi.MinimalApis.Payment
{
	public static class SinglePaymentApis
	{
		public static RouteGroupBuilder MapSinglePaymentApis(this RouteGroupBuilder group)
		{
			group
				.MapPost(RouteTemplates.Create, async Task<ApiResponse<Guid>> (
					IIdempotencyService idempotencyService,
					IHttpContextAccessor contextAccessor,
					ISingleOrderApplication singleorderApp,
					[FromBody] CreateSingleOrderDto dto) =>
				{
					try
					{
						var appResponse = await singleorderApp.CreateAsync(dto);
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
				.WithDisplayName("CreateSinglePayment")
				.WithSummary("ایجاد دستور پرداخت تکی جدید")
				.WithDescription("این متد یک دستور پرداخت تکی جدید با توجه به حساب پرداختی با وضعیت پیش نویس ایجاد می کند")
				.RequireIdempotency()
				.Produces(StatusCodes.Status201Created)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("addTransaction", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromBody] SingleTransactionDto dto) =>
				{
					try
					{
						var appResponse = await singleorderApp.AddTransaction(dto);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("AddTransaction")
				.WithSummary("افزودن جزییات تراکنش به دستور پرداخت تکی")
				.WithDescription("این متد جزییات تراکنش را به دستور پرداخت پیش نویس شده با توجه به شناسه آن اضافه می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapDelete("transaction/{OrderId:guid}/remove", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromRoute] Guid OrderId) =>
				{
					try
					{
						var appResponse = await singleorderApp.RemoveTransaction(OrderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("RemoveTransaction")
				.WithSummary("حذف جزییات تراکنش از دستور پرداخت تکی")
				.WithDescription("این متد جزییات تراکنش را از دستور پرداخت پیش نویس شده با توجه به شناسه آن حذف می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("finalize/{OrderId:guid}", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromRoute] Guid OrderId) =>
				{
					try
					{
						var appResponse = await singleorderApp.FinalizeOrder(OrderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("FinalizeOrder")
				.WithSummary("نهایی سازی دستور پرداخت تکی")
				.WithDescription("این متد دستور پرداخت پیش نویس شده با توجه به شناسه آن را نهایی می کند و وضعیت آن را به آماده به ارسال تغییر می دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("send/{OrderId:guid}", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromRoute] Guid OrderId) =>
				{
					try
					{
						var appResponse = await singleorderApp.SendOrderAsync(OrderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("SendOrder")
				.WithSummary("ارسال دستور پرداخت تکی")
				.WithDescription("این متد دستور پرداخت آماده به ارسال شده با توجه به شناسه آن را به سامانه بانکی جهت پردازش ارسال می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("inquiry/{OrderId:guid}", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromRoute] Guid OrderId) =>
				{
					try
					{
						var appResponse = await singleorderApp.InquiryPaymentOrder(OrderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("InquiryPaymentOrder")
				.WithSummary("استعلام وضعیت دستور پرداخت تکی")
				.WithDescription("این متد وضعیت دستور پرداخت ارسال شده با توجه به شناسه آن را از سامانه بانکی استعلام می کند")
				.Produces(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("report/{OrderId}", async Task<IResult> (ISingleOrderApplication singleorderApp, [FromRoute] string OrderId) =>
				{
					try
					{
						var appResponse = await singleorderApp.ReportAsync(OrderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("Report")
				.WithSummary("گزارش دستور پرداخت تکی")
				.WithDescription("این متد گزارش دستور پرداخت با توجه به شماره پرداخت را ارائه می دهد")
				.Produces<SingleOrderReportDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("getActiveAccounts", async Task<ApiResponse<List<AccountInfoDto>>> (ISingleOrderApplication singleorderApp) =>
				{
					try
					{
						var appResponse = await singleorderApp.GetActiveAccounts();
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
				.WithApiResponse<List<AccountInfoDto>>()
				.WithDisplayName("getActiveAccounts")
				.WithSummary("واگشی حساب های فعال دارای سرویس پرداخت تکی")
				.WithDescription("این متد حساب های فعالی که دارای سرویس پرداخت تکی هستند را خروجی می دهد")
				.Produces<List<AccountInfoDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			return group;
		}



	}
}
