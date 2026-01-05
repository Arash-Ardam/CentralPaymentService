using Domain.Banking.Bank;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations
{
	internal class BankDataConfiguration : IEntityTypeConfiguration<Bank>
	{
		public void Configure(EntityTypeBuilder<Bank> builder)
		{
			throw new NotImplementedException();
		}
	}
}
