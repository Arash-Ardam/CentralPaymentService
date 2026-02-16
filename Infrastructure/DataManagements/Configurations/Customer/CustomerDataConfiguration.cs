using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Customer
{
	internal class CustomerDataConfiguration : IEntityTypeConfiguration<Domain.Customer.Customer>
	{
		public void Configure(EntityTypeBuilder<Domain.Customer.Customer> builder)
		{
			builder.ToTable("Customers");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.IsEnable)
				.HasDefaultValue(true);

			builder.Property(x => x.TenantName)
				.IsRequired();

			builder.Property(x => x.ConnectionString)
				.HasDefaultValue(string.Empty);

			builder.OwnsOne(
				x => x.Info,
				info =>
				{
					info.Property(inf => inf.FirstName)
						.HasMaxLength(50);
						
					info.Property(inf => inf.LastName)
						.HasMaxLength(50);

					info.Property(inf => inf.NationalCode)
						.HasMaxLength(50);
				});
		}
	}
}
