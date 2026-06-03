using Application.Abstractions.Services;
using Application.Accounting.AccountApp.Services;
using Application.Accounting.BankApp.Services;
using Application.Accounting.CustomerApp.Services;
using Application.Administration.Services;
using Application.OrderManagement.Services;
using Infrastructure.Mappers;
using Infrastructure.Services.ApplicationServices;
using Infrastructure.Services.ApplicationServices.EventServices;
using Infrastructure.Services.ApplicationServices.QueryServices;
using Infrastructure.Services.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
	public static class ServiceConfiguration
	{
		public static void AddInfraServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Register Application related services
			services.AddScoped<IAccountQueryService, AccountQueryService>();
			services.AddScoped<ICustomerQueryService, CustomerQueryService>();
			services.AddScoped<ICustomerEventService, CustomerEventService>();
			services.AddScoped<IBankQueryService, BankQueryService>();
			services.AddScoped<IOrderEventService, OrderEventService>();
			services.AddScoped<IOrderReportService, OrderReportService>();
			services.AddScoped<IAdminOrderReportService, AdminOrderReportService>();

			// added tools services
			services.AddScoped<IUnitOfWork, DbUnitOfWork>();
			
			services.AddHostedService<ReportEventBackgroundService>();
			services.AddHostedService<CustomerEventsBackgroundService>();

			services.AddAutoMapper(config => config.AddMaps(typeof(MapperBase).Assembly));
		}
	}
}
