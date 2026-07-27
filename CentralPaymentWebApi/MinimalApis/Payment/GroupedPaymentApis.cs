using Application.Abstractions;
using Application.Accounting.AccountApp.Dtos;
using Application.OrderManagement;
using Application.OrderManagement.Dtos.GroupedOrder;
using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Configurations.EndpointsFilter;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Payment
{
	public static class GroupedPaymentApis
	{
		public static RouteGroupBuilder MapGroupedPaymentApis(this RouteGroupBuilder group)
		{
			group
				.MapPost(RouteTemplates.Create, async Task<ApiResponse<Guid>> (IGroupedOrderApplication app, [FromBody] CreateGroupedOrderDto dto) =>
				{
					try
					{
						var appResponse = await app.CreateAsync(dto);
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
				.WithDisplayName("CreateGroupedPayment")
				.WithSummary("ایجاد دستور پرداخت گروهی")
				.WithDescription("این متد دستور پرداخت گروهی جدید بر اساس شناسه حساب پرداخت ایجاد می کند")
				.Produces<Guid>(StatusCodes.Status201Created)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);


			group
				.MapPost("addTransactions", async Task<ApiResponse> (IGroupedOrderApplication app, [FromBody] AddGroupedTransactionDto dto) =>
				{
					try
					{
						var appResponse = await app.AddTransactions(dto);
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
				.WithDisplayName("AddGroupedTransactions")
				.WithSummary("افزودن تراکنش های گروهی")
				.WithDescription("این متد تراکنش های گروهی را به دستور پرداخت اضافه می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/transacions/{transactionId:guid}/remove", async Task<ApiResponse> (IGroupedOrderApplication app, [FromRoute] Guid orderId, [FromRoute] Guid transactionId) =>
				{
					try
					{
						var appResponse = await app.RemoveTransaction(orderId, transactionId);
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
				.WithDisplayName("RemoveGroupedTransaction")
				.WithSummary("حذف تراکنش گروهی")
				.WithDescription("این متد تراکنش گروهی را از دستور پرداخت حذف می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/finalize", async Task<ApiResponse> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.FinalizeOrder(orderId);
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
				.WithDisplayName("FinalizeGroupedOrder")
				.WithSummary("نهایی سازی به دستور پرداخت گروهی")
				.WithDescription("این متد دستور پرداخت گروهی را برای ارسال به بانک نهایی می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/send", async Task<ApiResponse> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.SendOrderAsync(orderId);
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
				.WithDisplayName("SendToBank")
				.WithSummary("ارسال دستور پرداخت گروهی به بانک جهت پردازش")
				.WithDescription("این متد دستور پرداخت گروهی را پس از نهایی سازی به بانک ارسال می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/inquiry", async Task<ApiResponse> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.InquiryPaymentOrder(orderId);
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
				.WithDisplayName("InquiryGroupedOrder")
				.WithSummary("استعلام وضعیت دستور پرداخت گروهی")
				.WithDescription("این متد وضعیت دستور پرداخت گروهی را از بانک استعلام می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/transacions/{transactionId:guid}/inquiry", async Task<ApiResponse> (IGroupedOrderApplication app, [FromRoute] Guid orderId, [FromRoute] Guid transactionId) =>
			{
				try
				{
					var appResponse = await app.InquiryPaymentTransaction(orderId, transactionId);
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
			.WithDisplayName("InquiryGroupedTransaction")
			.WithSummary("استعلام وضعیت تراکنش گروهی")
			.WithDescription("این متد وضعیت تراکنش گروهی را از بانک استعلام می کند")
			.Produces(StatusCodes.Status202Accepted)
			.Produces<string>(StatusCodes.Status400BadRequest)
			.Produces<string>(StatusCodes.Status404NotFound)
			.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("{orderId}/report", async Task<ApiResponse<GroupedOrderReportDto>> (IGroupedOrderApplication app, [FromRoute] string orderId) =>
				{
					try
					{
						var appResponse = await app.ReportOrderAsync(orderId);
						return appResponse.HandleApiResponse();
					}
					catch (Exception ex)
					{
						return new ApiResponse<GroupedOrderReportDto>
						{
							HttpResult = Results.InternalServerError(ex.Message),
							AppResponse = new ApplicationResponse<GroupedOrderReportDto>
							{
								IsSuccess = false,
								Message = ex.Message,
								Status = ApplicationResultStatus.Exception
							}
						};
					}
				})
				.WithApiResponse<GroupedOrderReportDto>()
				.WithDisplayName("ReportGroupedOrder")
				.WithSummary("گزارش دستور پرداخت گروهی")
				.WithDescription("این متد گزارش اخرین وضعیت دستور پرداخت گروهی را به همراه تراکنش ها خروجی می دهد")
				.Produces<GroupedOrderReportDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group.MapGet("{orderId}/transacions/{transactionOrderId}/report", async Task<ApiResponse<GroupedOrderTransactionReportDto>> (IGroupedOrderApplication app, [FromRoute] string orderId, [FromRoute] string transactionOrderId) =>
			{
				try
				{
					var appResponse = await app.ReportTrasnactionAsync(orderId, transactionOrderId);
					return appResponse.HandleApiResponse();
				}
				catch (Exception ex)
				{
					return new ApiResponse<GroupedOrderTransactionReportDto>
					{
						HttpResult = Results.InternalServerError(ex.Message),
						AppResponse = new ApplicationResponse<GroupedOrderTransactionReportDto>
						{
							IsSuccess = false,
							Message = ex.Message,
							Status = ApplicationResultStatus.Exception
						}
					};
				}
			})
			.WithApiResponse<GroupedOrderTransactionReportDto>()
			.WithDisplayName("ReportGroupedTransaction")
			.WithSummary("گزارش تراکنش پرداخت گروهی")
			.WithDescription("این متد گزارش اخرین وضعیت تراکنش پرداخت گروهی را خروجی می دهد")
			.Produces<GroupedOrderTransactionReportDto>(StatusCodes.Status200OK)
			.Produces<string>(StatusCodes.Status400BadRequest)
			.Produces<string>(StatusCodes.Status404NotFound)
			.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("getActiveAccounts", async Task<ApiResponse<List<AccountInfoDto>>> (IGroupedOrderApplication app) =>
				{
					try
					{
						var appResponse = await app.GetActiveAccounts();
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
				.WithDisplayName("Report")
				.WithSummary("واگشی حساب های فعال دارای سرویس پرداخت گروهی")
				.WithDescription("این متد حساب های فعالی که دارای سرویس پرداخت گروهی هستند را خروجی می دهد")
				.Produces<List<AccountInfoDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			return group;
		}

	}
}
