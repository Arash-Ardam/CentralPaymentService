using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.DataManagements.MultiTenancyServices
{
	internal sealed class TenantInitializerHostedService : IHostedService
	{
		private readonly ITenantRegistryService tenantRegistryService;
		public TenantInitializerHostedService(ITenantRegistryService tenantRegistryService)
		{
			this.tenantRegistryService = tenantRegistryService ?? throw new ArgumentNullException(nameof(tenantRegistryService));
		}


		public Task StartAsync(CancellationToken cancellationToken) => tenantRegistryService.RefreshAsync();


		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
