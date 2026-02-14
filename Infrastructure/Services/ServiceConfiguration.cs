using Application.Accounting.AccountApp.Services;
using Application.Accounting.BankApp.Services;
using Application.Accounting.CustomerApp.Services;
using Infrastructure.Services.ApplicationServices.QueryServices;
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
			services.AddScoped<IBankQueryService, BankQueryService>();
		}
	}
}
