using Application;
using CentralPaymentWebApi.Configurations.Identity;
using CentralPaymentWebApi.Configurations.OpenApi;
using CentralPaymentWebApi.Middlewares;
using Infrastructure.DataManagements;
using Infrastructure.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(opts =>
	{
		opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
	});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataManagements(builder.Configuration);
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddApplications();

builder.AddApiAuthentication();
builder.AddApiAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalar();
}

app.UseMiddleware<TenantContextMiddleware>();

app.MapControllers();

app.Run();
