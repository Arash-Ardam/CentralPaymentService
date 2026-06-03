using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.MultiTenancyServices.TenantResolver;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.MultiTenancyServices.TenantDbContextFactory
{
	internal class TenantDbContextFactory
		: ITenantDbContextFactory
	{
		private readonly ITenantResolver _resolver;

		public TenantDbContextFactory(
			ITenantResolver resolver)
		{
			_resolver = resolver;
		}

		public TenantEfCoreDbContext Create()
		{
			var connectionString = _resolver.Resolve();

			var options =
				new DbContextOptionsBuilder<TenantEfCoreDbContext>()
					.UseSqlServer(connectionString)
					.Options;

			return new TenantEfCoreDbContext(options);
		}
	}
}
