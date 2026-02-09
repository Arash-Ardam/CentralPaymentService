using Application.Accounting.AccountApp;
using Application.Accounting.BankApp;
using Application.Accounting.CustomerApp;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public static class Registrations
	{
		public static void AddApplications(this IServiceCollection services)
		{
			// Register Tools
			services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Registrations).Assembly));
			services.AddAutoMapper(config => config.AddMaps(typeof(Registrations).Assembly));

			// Register applications
			services.AddScoped<IBankApplication, BankApplication>();
			services.AddScoped<ICustomerApplication, CustomerApplication>();
			services.AddScoped<IAccountApplication, AccountApplication>();
		}
	}
}
