using Application.Abstractions.Services;
using Application.Accounting.AccountApp.Services;
using Application.Accounting.BankApp.Services;
using Application.Accounting.CustomerApp.Services;
using Application.Administration.Services;
using Application.OrderManagement.Ports.SinglePaymentServices;
using Application.OrderManagement.Services;
using Infrastructure.Mappers;
using Infrastructure.Services.ApplicationServices;
using Infrastructure.Services.ApplicationServices.EventServices;
using Infrastructure.Services.ApplicationServices.PaymentServices.PSP;
using Infrastructure.Services.ApplicationServices.QueryServices;
using Infrastructure.Services.BackgroundServices;
using Infrastructure.Services.Idempotency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
	public static class ServiceConfiguration
	{
		public static void AddInfraServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Register Application related services

			#region Query Services
			services.AddScoped<IAccountQueryService, AccountQueryService>();
			services.AddScoped<ICustomerQueryService, CustomerQueryService>();
			services.AddScoped<IBankQueryService, BankQueryService>();
			#endregion

			#region Event Services
			//services.AddScoped<ICustomerEventService, CustomerEventService>();
			//services.AddScoped<IOrderEventService, OrderEventService>();
			services.AddScoped<IOutboxMessageService, OutboxMessageService>();
			#endregion

			#region Report Services
			services.AddScoped<IOrderReportService, OrderReportService>();
			services.AddScoped<IAdminOrderReportService, AdminOrderReportService>();
			#endregion

			#region PSP Payment Services
			services.AddScoped<IPSPPaymentService, SamanPSPService>();
			#endregion

			#region Tools Services

			services.AddScoped<IUnitOfWork, DbUnitOfWork>();
			services.AddScoped<IIdempotencyService, IdempotencyService>();

			#endregion
			#region BackgroundServices
			services.AddHostedService<ReportEventBackgroundService>();
			services.AddHostedService<CustomerEventsBackgroundService>();
			services.AddHostedService<IdempotencyExpirationBackgroundService>();
			#endregion

			services.AddAutoMapper(config => config.AddMaps(typeof(MapperBase).Assembly));
		}
	}
}
