using Domain.Order;
using Infrastructure.DataManagements.Configurations.Order;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Abstractions.ORMs
{
	internal class TenantEfCoreDbContext : DbContext
	{
		public TenantEfCoreDbContext(DbContextOptions<TenantEfCoreDbContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders => Set<Order>();
		public DbSet<OrderEventModel> OrderEvents => Set<OrderEventModel>();
		public DbSet<SingleOrderReportModel> SingleOrderReports => Set<SingleOrderReportModel>();
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new GroupedTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new SingleTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new OrderDataConfiguration());
			modelBuilder.ApplyConfiguration(new OrderEventsConfiguration());
			modelBuilder.ApplyConfiguration(new OrderReportConfiguration());

			base.OnModelCreating(modelBuilder);
		}

		internal void SetConnectionString(string connectionString)
		{
			Database.SetConnectionString(connectionString);
			ChangeTracker.Clear();
		}
	}
}
