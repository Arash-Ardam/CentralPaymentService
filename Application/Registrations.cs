using Application.Accounting.BankApp;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public static class Registrations
	{
		public static void AddApplications(this IServiceCollection services)
		{
			// Register Tools
			services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Registrations).Assembly));

			// Register applications
			services.AddScoped<IBankApplication, BankApplication>();
		}
	}
}
