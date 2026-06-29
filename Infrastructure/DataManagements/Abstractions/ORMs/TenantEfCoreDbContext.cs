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
		public DbSet<SingleOrderReportModel> SingleOrderReports => Set<SingleOrderReportModel>();
		public DbSet<GroupedOrderReportModel> GroupedOrderReports => Set<GroupedOrderReportModel>();
		public DbSet<GroupedOrderTransactionReportModel> GroupedTransactionsReports => Set<GroupedOrderTransactionReportModel>();
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new GroupedTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new SingleTransactionConfiguration());
			modelBuilder.ApplyConfiguration(new OrderDataConfiguration());
			modelBuilder.ApplyConfiguration(new SingleOrderReportConfiguration());
			modelBuilder.ApplyConfiguration(new GroupedOrderReportConfiguration());
			modelBuilder.ApplyConfiguration(new GroupedOrderTranactionsReportConfiguration());

			base.OnModelCreating(modelBuilder);
		}

		internal void SetConnectionString(string connectionString)
		{
			Database.SetConnectionString(connectionString);
			ChangeTracker.Clear();
		}
	}
}
