using Domain.Banking.Bank.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.DataManagements.Configurations.Bank
{
	internal class BankDataConfiguration : IEntityTypeConfiguration<Domain.Banking.Bank.Bank>
	{
		public void Configure(EntityTypeBuilder<Domain.Banking.Bank.Bank> builder)
		{
			builder.ToTable("Banks");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(x => x.isEnable)
				.HasDefaultValue(true);

			builder.Property(x => x.Code)
				.HasDefaultValue(BankCode.None);

			builder.Property(x => x.ServiceTypes)
				.HasConversion(
				 v => JsonSerializer.Serialize(v),
				 v => JsonSerializer.Deserialize<List<ServiceTypes>>(v) ?? new List<ServiceTypes>());
		}
	}
}
