using Domain.Banking.Account;
using Domain.Banking.Bank;
using Domain.Customer;
using Infrastructure.DataManagements.Configurations.Account;
using Infrastructure.DataManagements.Configurations.Bank;
using Infrastructure.DataManagements.Configurations.Customer;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Abstractions
{
	internal class AdminEfCoreDbContext : DbContext
	{
		public AdminEfCoreDbContext(DbContextOptions<AdminEfCoreDbContext> options) : base(options)
		{
		}

		public DbSet<Bank> Banks => Set<Bank>();
		public DbSet<Account> Accounts => Set<Account>();
		public DbSet<Customer> Customers => Set<Customer>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new BankDataConfiguration());
			modelBuilder.ApplyConfiguration(new CustomerDataConfiguration());
			modelBuilder.ApplyConfiguration(new AccountDataConfiguration());

			base.OnModelCreating(modelBuilder);
		}


	}
}
