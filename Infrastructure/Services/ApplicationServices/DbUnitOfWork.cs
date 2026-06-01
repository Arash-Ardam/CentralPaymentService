using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions.ORMs;

namespace Infrastructure.Services.ApplicationServices
{
	internal sealed class DbUnitOfWork : IUnitOfWork
	{
		private readonly AdminEfCoreDbContext _adminDb;
		private readonly TenantEfCoreDbContext _tenantDb;
		public DbUnitOfWork(AdminEfCoreDbContext adminDb, TenantEfCoreDbContext tenantDb)
		{
			_adminDb = adminDb ?? throw new ArgumentNullException(nameof(adminDb));
			_tenantDb = tenantDb ?? throw new ArgumentNullException(nameof(tenantDb));
		}
		public Task SaveAdminChangesAsync(CancellationToken cancellationToken = default) => _adminDb.SaveChangesAsync(cancellationToken);

		public Task SaveTenantChangesAsync(CancellationToken cancellationToken = default) => _tenantDb.SaveChangesAsync(cancellationToken);
	}
}
