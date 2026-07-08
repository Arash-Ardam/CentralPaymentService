using Application.Accounting.CustomerApp;
using Application.Accounting.CustomerApp.Dtos;
using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.MinimalApis.Accounting
{
	public static class CustomerApis
	{
		public static RouteGroupBuilder MapCustomerApis(this RouteGroupBuilder group)
		{
			group
				.MapPost(RouteTemplates.Create, async Task<IResult> (ICustomerApplication customerApp, [FromBody] CreateCustomerDto dto) =>
				{
					try
					{
						var appResponse = await customerApp.CreateAsync(dto);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.BadRequest(ex.Message);
					}
				})
				.WithDisplayName("CreateCustomer")	 	
				.WithSummary("ایجاد مشتری جدید")
				.WithDescription("ایجاد مشتری جدید در سیستم")
				.Produces(statusCode: StatusCodes.Status201Created)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest);

			group
				.MapPost("setSettings", async Task<IResult> (ICustomerApplication customerApp, [FromBody] InformationDto dto) =>
				{
					try
					{
						var appResponse = await customerApp.SetCustomerSettings(dto);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.BadRequest(ex.Message);
					}
				})
				.WithDisplayName("SetSettings")
				.WithSummary("به‌روز رسانی تنظیمات مشتری")
				.WithDescription("تنظیمات مشتری را به‌روز می‌کند")
				.Produces(statusCode: StatusCodes.Status202Accepted)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest);

			group
				.MapPost("changeStatus", async Task<IResult> (ICustomerApplication customerApp, [FromQuery] Guid customerId, [FromQuery] bool status) =>
				{
					try
					{
						var appResponse = await customerApp.ChangeStatus(customerId, status);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.BadRequest(ex.Message);
					}
				})
				.WithDisplayName("ChangeStatus")
				.WithSummary("تغییر وضعیت مشتری")
				.WithDescription("وضعیت مشتری را تغییر می‌دهد")
				.Produces(statusCode: StatusCodes.Status202Accepted)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest);

			group
				.MapGet(RouteTemplates.Get, async Task<IResult> (ICustomerApplication customerApp, [FromRoute] Guid id) =>
				{
					try
					{
						var appResponse = await customerApp.GetAsync(id);
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.BadRequest(ex.Message);
					}
				})
				.WithDisplayName("GetInfo")
				.WithSummary("دریافت اطلاعات مشتری بر اساس شناسه")
				.WithDescription("اطلاعات مشتری را باز می‌گرداند")
				.Produces<CustomerInfoDto>(statusCode: StatusCodes.Status200OK)
				.Produces(statusCode: StatusCodes.Status404NotFound)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest);


			group
				.MapGet(RouteTemplates.GetAll, async Task<IResult> (ICustomerApplication customerApp) =>
				{
					try
					{
						var appResponse = await customerApp.GetAllAsync();
						return appResponse.HandleOutput();
					}
					catch (Exception ex)
					{
						return Results.BadRequest(ex.Message);
					}
				})
				.WithDisplayName("GetAllInfo")
				.WithSummary("دریافت اطلاعات تمامی مشتریان")
				.WithDescription("لیست تمام مشتریان را باز می‌گرداند")
				.Produces<List<CustomerInfoDto>>(statusCode: StatusCodes.Status200OK)
				.Produces(statusCode: StatusCodes.Status404NotFound)
				.Produces<string>(statusCode: StatusCodes.Status400BadRequest);

			return group;
		}

	}
}
