using Application;
using CentralPaymentWebApi.Middlewares;
using Infrastructure.DataManagements;
using Infrastructure.Services;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(opts =>
	{
		opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase,allowIntegerValues: true)); 
	}); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataManagements(builder.Configuration);
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddApplications();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<TenantContextMiddleware>();

app.MapControllers();

app.Run();
