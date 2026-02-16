using Application.OrderManagement.Services;
using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Domain.Order;
using Infrastructure.DataManagements.Abstractions;
using Infrastructure.DataManagements.MultiTenancyServices;
using Infrastructure.DataManagements.MultiTenancyServices.TenantResolver;
using Infrastructure.DataManagements.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataManagements
{
	public static class DataRegistrations
	{
		public static void AddDataManagements(this IServiceCollection services, IConfiguration configuration)
		{
			// Register ORM Tools
			var efCoreConfig = configuration.GetRequiredSection(nameof(ORMToolsOptions)).Get<ORMToolsOptions>();

			// EfCore : Admin
			if (efCoreConfig.EfCore.isEnable)
			{
				services.AddDbContext<AdminEfCoreDbContext>(options =>
				options.UseSqlServer(efCoreConfig.EfCore.BaseConnectionString,
				sqlOptions =>
				{
					sqlOptions.MigrationsAssembly(typeof(AdminEfCoreDbContext).Assembly.FullName);
					sqlOptions.EnableRetryOnFailure(
						maxRetryCount: efCoreConfig.EfCore.RetryCount,
						maxRetryDelay: efCoreConfig.EfCore.RetryDelay,
						errorNumbersToAdd: null);
				}),
				contextLifetime: ServiceLifetime.Scoped);
			}

			// EfCore : Tenant

			services.AddScoped<ITenantContext, TenantContext>();
			services.AddScoped<ITenantResolver, TenantResolver>();

			if (efCoreConfig.EfCore.isEnable)
			{
				services.AddDbContext<TenantEfCoreDbContext>((serviceProvider, options) =>
				{
					options.UseSqlServer("connectionString",
					sqlOptions =>
					{
						sqlOptions.MigrationsAssembly(typeof(TenantEfCoreDbContext).Assembly.FullName);
						sqlOptions.EnableRetryOnFailure(
							maxRetryCount: efCoreConfig.EfCore.RetryCount,
							maxRetryDelay: efCoreConfig.EfCore.RetryDelay,
							errorNumbersToAdd: null);
					});
				},
				contextLifetime: ServiceLifetime.Scoped);
			}

			// Register ORM Configs as Options : using in multiTenancy
			services.AddOptions<ORMToolsOptions>()
					.Configure<IConfiguration>((options, configs) => configs.GetSection(nameof(ORMToolsOptions)).Bind(options));

			// Register repositories
			services.AddScoped<IBankRepository, BankRepository>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
		}
	}
}
