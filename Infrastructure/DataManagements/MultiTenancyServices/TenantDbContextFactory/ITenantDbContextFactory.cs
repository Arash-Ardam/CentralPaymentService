using Infrastructure.DataManagements.Abstractions.ORMs;

namespace Infrastructure.DataManagements.MultiTenancyServices.TenantDbContextFactory
{
	internal interface ITenantDbContextFactory
	{
		public TenantEfCoreDbContext Create();
	}
}
