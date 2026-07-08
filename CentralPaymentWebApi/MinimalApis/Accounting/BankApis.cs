using Application.Accounting.BankApp;
using Application.Accounting.BankApp.Dtos;
using CentralPaymentWebApi.Abstractions;
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
				 .MapPost(RouteTemplates.Create, async Task<IResult> (IBankApplication bankApp, [FromBody] CreateBankDto dto) =>
				 {
					 try
					 {
						 if (dto.BankCode == null)
							 return Results.BadRequest("BankCode is null");

						 var appResponse = await bankApp.CreateAsync(dto);
						 return appResponse.HandleOutput();
					 }
					 catch (Exception ex)
					 {
						 return Results.InternalServerError(ex.Message);
					 }
				 })
				 .WithDisplayName("CreateBank")
				 .WithSummary("ایجاد بانک جدید")
				 .WithDescription("ایجاد بانک جدید در سیستم")
				 .Produces(statusCode: StatusCodes.Status201Created)
				 .Produces<string>(statusCode: StatusCodes.Status400BadRequest);


			group
				.MapGet(RouteTemplates.Get, async Task<IResult> (IBankApplication bankApp, Guid id) =>
				{
					try
					{
						var appResponse = await bankApp.GetAsync(id);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("GetInfo")
				.WithSummary("دریافت اطلاعات بانک")
				.WithDescription("این متد اطلاعات بانک مورد نظر را بر اساس شناسه ارسالی خروجی می دهد")
				.Produces<BankInfoDto>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapGet(RouteTemplates.GetAll, async Task<IResult> (IBankApplication bankApp) =>
				{
					try
					{
						var appResponse = await bankApp.GetAll();
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("GetAllInfo")
				.WithSummary("دریافت اطلاعات تمامی بانک ها")
				.WithDescription("این متد اطلاعات تمامی بانک های موجود در سیستم را خروجی می دهد")
				.Produces<List<BankInfoDto>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("assignServices", async Task<IResult> (IBankApplication bankApp, [FromBody] AssignServiceDto dto) =>
				{
					try
					{
						if (dto.ServiceTypes.Contains(ServiceTypes.None))
							return Results.BadRequest("Invalid service types");

						var appResponse = await bankApp.AssignPaymentServices(dto.BankId, dto.ServiceTypes);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
				.WithDisplayName("AssignServices")
				.WithSummary("اختصاص سرویس های پرداخت به بانک")
				.WithDescription("این متد سرویس های پرداخت را به بانک مورد نظر اختصاص می دهد")
				.Produces(StatusCodes.Status202Accepted)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.Produces<string>(StatusCodes.Status500InternalServerError);

			group
				.MapPost("changeStatus", async Task<IResult> (IBankApplication bankApp, [FromBody] ChangeBankStatusDto dto) =>
				{
					try
					{
						var appResponse = await bankApp.ChangeStatusAsync(dto.Id, dto.Status);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.InternalServerError(ex.Message);
					}
				})
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
