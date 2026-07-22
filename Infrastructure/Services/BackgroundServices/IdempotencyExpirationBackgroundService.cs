using Application.OrderManagement.Services;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Infrastructure.DataManagements.MultiTenancyServices.TenantResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.BackgroundServices
{
	internal sealed class IdempotencyExpirationBackgroundService : BackgroundService
	{
		public IdempotencyExpirationBackgroundService(IServiceScopeFactory serviceProvider)
		{
			_serviceFactory = serviceProvider;
		}

		private readonly IServiceScopeFactory _serviceFactory;


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = _serviceFactory.CreateScope();

				var tenantRegistryService = scope.ServiceProvider.GetRequiredService<ITenantRegistryService>();
				var tenantDb =scope.ServiceProvider.GetRequiredService<TenantEfCoreDbContext>();
				var tenantResolver = scope.ServiceProvider.GetRequiredService<ITenantResolver>();
				var efcoreOptions = scope.ServiceProvider.GetRequiredService<IOptions<ORMToolsOptions>>();
				var connectionString = string.Empty;

				foreach (var tenant in tenantRegistryService.GetAll())
				{
					tenantDb.SetConnectionString(
					string.IsNullOrWhiteSpace(tenant.ConnectionString)
						   ? string.Format(efcoreOptions.Value.EfCore.TenantConnectionString, tenant.Name)
						   : tenant.ConnectionString);

					await tenantDb.IdempotencyRequests
						.Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow)
						.ExecuteDeleteAsync(stoppingToken);
				}
				await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
			}
		}
	}
}
