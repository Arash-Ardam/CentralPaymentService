using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Customer
{
	internal class CustomerEventDataConfiguration : IEntityTypeConfiguration<CustomerEventModel>
	{
		public void Configure(EntityTypeBuilder<CustomerEventModel> builder)
		{
			builder.Property(prop => prop.TenantName)
				.IsRequired()
				.HasMaxLength(50);

			builder.Property(prop => prop.Type)
				.IsRequired();

			builder.Property(prop => prop.Payload)
				.IsRequired(false);

			builder.Property(prop => prop.ConnectionString)
				.IsRequired(false);

			builder.Property(prop => prop.IsProccessed)
				.HasDefaultValue(false);

		}
	}
}
