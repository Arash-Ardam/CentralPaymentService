using Domain.Banking.Account.Services;
using Domain.Banking.Bank.Services;
using Domain.Customer.Services;
using Infrastructure.Services.DomainServices.Account;
using Infrastructure.Services.DomainServices.Bank;
using Infrastructure.Services.DomainServices.Customer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
	public static class ServiceConfiguration
	{
		public static void AddInfraServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Register Domain related services
			services.AddScoped<IBankIdentifierService, BankIdentifierService>();
			services.AddScoped<ICustomerIdentifierService, CustomerIdentifierService>();
			services.AddScoped<IAccountIdentifierService, AccountIdentifierService>();
		}
	}
}
