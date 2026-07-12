using Application.Accounting.AccountApp.Dtos;
using Application.OrderManagement;
using Application.OrderManagement.Dtos.GroupedOrder;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Payment
{
	public static class GroupedPaymentApis
	{
		public static RouteGroupBuilder MapGroupedPaymentApis(this RouteGroupBuilder group)
		{
			group
				.MapPost(RouteTemplates.Create, async Task<IResult> (IGroupedOrderApplication app, [FromBody] CreateGroupedOrderDto dto) =>
				{
					try
					{
						var appResponse = await app.CreateAsync(dto);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("CreateGroupedPayment")
				.WithSummary("ایجاد دستور پرداخت گروهی")
				.WithDescription("این متد دستور پرداخت گروهی جدید بر اساس شناسه حساب پرداخت ایجاد می کند")
				.Produces<Guid>(StatusCodes.Status201Created)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);


			group
				.MapPost("addTransactions", async Task<IResult> (IGroupedOrderApplication app, [FromBody] AddGroupedTransactionDto dto) =>
				{
					try
					{
						var appResponse = await app.AddTransactions(dto);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("AddGroupedTransactions")
				.WithSummary("افزودن تراکنش های گروهی")
				.WithDescription("این متد تراکنش های گروهی را به دستور پرداخت اضافه می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/transacions/{transactionId:guid}/remove", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] Guid orderId, [FromRoute] Guid transactionId) =>
				{
					try
					{
						var appResponse = await app.RemoveTransaction(orderId, transactionId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("RemoveGroupedTransaction")
				.WithSummary("حذف تراکنش گروهی")
				.WithDescription("این متد تراکنش گروهی را از دستور پرداخت حذف می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/finalize", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.FinalizeOrder(orderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("FinalizeGroupedOrder")
				.WithSummary("نهایی سازی به دستور پرداخت گروهی")
				.WithDescription("این متد دستور پرداخت گروهی را برای ارسال به بانک نهایی می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/send", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.SendOrderAsync(orderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("SendToBank")
				.WithSummary("ارسال دستور پرداخت گروهی به بانک جهت پردازش")
				.WithDescription("این متد دستور پرداخت گروهی را پس از نهایی سازی به بانک ارسال می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/inquiry", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] Guid orderId) =>
				{
					try
					{
						var appResponse = await app.InquiryPaymentOrder(orderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("InquiryGroupedOrder")
				.WithSummary("استعلام وضعیت دستور پرداخت گروهی")
				.WithDescription("این متد وضعیت دستور پرداخت گروهی را از بانک استعلام می کند")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("{orderId:guid}/transacions/{transactionId:guid}/inquiry", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] Guid orderId, [FromRoute] Guid transactionId) =>
			{
				try
				{
					var appResponse = await app.InquiryPaymentTransaction(orderId, transactionId);
					return appResponse.HandleOutput();
				}
				catch (Exception ex)
				{
					return Results.InternalServerError(ex.Message);
				}
			})
			.WithDisplayName("InquiryGroupedTransaction")
			.WithSummary("استعلام وضعیت تراکنش گروهی")
			.WithDescription("این متد وضعیت تراکنش گروهی را از بانک استعلام می کند")
			.Produces(StatusCodes.Status202Accepted)
			.Produces<string>(StatusCodes.Status400BadRequest)
			.Produces<string>(StatusCodes.Status404NotFound)
			.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("{orderId}/report", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] string orderId) =>
				{
					try
					{
						var appResponse = await app.ReportOrderAsync(orderId);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("ReportGroupedOrder")
				.WithSummary("گزارش دستور پرداخت گروهی")
				.WithDescription("این متد گزارش اخرین وضعیت دستور پرداخت گروهی را به همراه تراکنش ها خروجی می دهد")
				.Produces<GroupedOrderReportDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status404NotFound)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group.MapGet("{orderId}/transacions/{transactionOrderId}/report", async Task<IResult> (IGroupedOrderApplication app, [FromRoute] string orderId, [FromRoute] string transactionOrderId) =>
			{
				try
				{
					var appResponse = await app.ReportTrasnactionAsync(orderId, transactionOrderId);
					return appResponse.HandleOutput();
				}
				catch (Exception ex)
				{
					return Results.InternalServerError(ex.Message);
				}
			})
			.WithDisplayName("ReportGroupedTransaction")
			.WithSummary("گزارش تراکنش پرداخت گروهی")
			.WithDescription("این متد گزارش اخرین وضعیت تراکنش پرداخت گروهی را خروجی می دهد")
			.Produces<GroupedOrderTransactionReportDto>(StatusCodes.Status200OK)
			.Produces<string>(StatusCodes.Status400BadRequest)
			.Produces<string>(StatusCodes.Status404NotFound)
			.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet("getActiveAccounts", async Task<IResult> (IGroupedOrderApplication app) =>
				{
					try
					{
						var appResponse = await app.GetActiveAccounts();
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
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
