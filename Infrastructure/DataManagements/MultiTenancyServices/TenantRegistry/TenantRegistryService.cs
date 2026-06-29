using Infrastructure.DataManagements.Abstractions.Dtos;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry
{
	internal class TenantRegistryService : ITenantRegistryService
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		public TenantRegistryService(IServiceScopeFactory serviceScopeFactory)
		{
			this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		}



		private List<TenantInfoDto> _tenants;



		public IReadOnlyList<TenantInfoDto> GetAll() => _tenants;

		public async Task RefreshAsync()
		{
			using var scope = serviceScopeFactory.CreateScope();
			var adminDb = scope.ServiceProvider.GetRequiredService<AdminEfCoreDbContext>();

			_tenants = await adminDb.Customers
				.AsNoTracking()
				.Select(x => new TenantInfoDto
				{
					Id = x.Id,
					Name = x.TenantName,
					ConnectionString = x.ConnectionString
				}).ToListAsync();
		}

		public TenantInfoDto? Find(Guid id) => _tenants.Find(x => x.Id == id);
	}
}
