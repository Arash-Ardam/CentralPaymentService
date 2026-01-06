using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;
using Infrastructure.DataManagements.Abstractions;
using Infrastructure.DataManagements.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataManagements
{
	public static class DataRegistrations
	{
		public static void AddDataManagements(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddDbContext<EfCoreDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("EfCoreConnectionStrings"),
				sqlOptions => sqlOptions.MigrationsAssembly(typeof(EfCoreDbContext).Assembly.FullName)));

			// Register repositories
			services.AddScoped<IBankRepository, BankRepository>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
		}
	}
}
