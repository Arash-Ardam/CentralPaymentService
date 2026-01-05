using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Order;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Abstractions
{
	internal class EfCoreDbContext : DbContext
	{
		public EfCoreDbContext(DbContextOptions<EfCoreDbContext> options) : base(options)
		{
		}

		public DbSet<Bank> Banks => Set<Bank>();
		public DbSet<Account> Accounts => Set<Account>();
		public DbSet<Order> Orders => Set<Order>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(EfCoreDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}


	}
}
