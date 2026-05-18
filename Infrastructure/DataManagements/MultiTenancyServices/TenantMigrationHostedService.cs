using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataManagements.MultiTenancyServices
{
	internal class TenantMigrationHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ORMToolsOptions _options;


		public TenantMigrationHostedService(IServiceProvider serviceProvider ,IOptions<ORMToolsOptions> options)
		{
			_serviceProvider = serviceProvider;
			_options = options.Value;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();

			var adminDb =
				scope.ServiceProvider
					.GetRequiredService<AdminEfCoreDbContext>();

			var tenants = await adminDb.Customers
				.AsNoTracking()
				.ToListAsync(cancellationToken);

			foreach (var tenant in tenants)
			{
				var builder =
					new DbContextOptionsBuilder<TenantEfCoreDbContext>();

				var connectionString = string.IsNullOrWhiteSpace(tenant.ConnectionString)
				? string.Format(_options.EfCore.TenantConnectionString,
					tenant.TenantName)
				: tenant.ConnectionString;

				builder.UseSqlServer(connectionString, sqlOptions =>
				{
					sqlOptions.MigrationsAssembly(typeof(TenantEfCoreDbContext).Assembly.FullName);
					sqlOptions.EnableRetryOnFailure(
						maxRetryCount: _options.EfCore.RetryCount,
						maxRetryDelay: _options.EfCore.RetryDelay,
						errorNumbersToAdd: null);
				});

				await using var tenantDb =
					new TenantEfCoreDbContext(builder.Options);

				await tenantDb.Database.MigrateAsync(cancellationToken);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
