using Domain.Order;
using Infrastructure.DataManagements.Configurations.Order;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Abstractions
{
	internal class TenantEfCoreDbContext : DbContext
	{
		public TenantEfCoreDbContext(DbContextOptions<TenantEfCoreDbContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders => Set<Order>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new GroupedTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new SingleTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new OrderDataConfiguration());

			base.OnModelCreating(modelBuilder);
		}

		internal void SetConnectionString(string connectionString)
		{
			Database.SetConnectionString(connectionString);
			ChangeTracker.Clear();
		}
	}
}
