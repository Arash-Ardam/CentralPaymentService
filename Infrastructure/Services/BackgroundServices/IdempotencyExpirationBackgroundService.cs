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

				var tenantRegistryService = scope.ServiceProvider.GetService<ITenantRegistryService>();
				var tenantDb =scope.ServiceProvider.GetService<TenantEfCoreDbContext>();
				var tenantResolver = scope.ServiceProvider.GetService<ITenantResolver>();
				var efcoreOptions = scope.ServiceProvider.GetService<IOptions<EfCoreOptions>>();
				var connectionString = string.Empty;

				foreach (var item in tenantRegistryService.GetAll())
				{
					connectionString = string.IsNullOrWhiteSpace(item.ConnectionString) ? string.Format(efcoreOptions.Value.TenantConnectionString,item.Name) : item.ConnectionString;
					tenantDb.SetConnectionString(connectionString);

					var expiredIdempotentRequests = tenantDb.IdempotencyRequests
					.Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow);

					if (expiredIdempotentRequests.Any())
						tenantDb.IdempotencyRequests.RemoveRange(expiredIdempotentRequests);
				}


			

				await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
			}
		}
	}
}
