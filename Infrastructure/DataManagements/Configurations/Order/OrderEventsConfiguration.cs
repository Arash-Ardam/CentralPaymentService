using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class OrderEventsConfiguration : IEntityTypeConfiguration<OrderEventModel>
	{
		public void Configure(EntityTypeBuilder<OrderEventModel> builder)
		{
			builder.ToTable("OrderEvents");
			builder.HasKey(e => e.Id);

			builder.Property(e => e.RetryCount)
				.HasDefaultValue(0);
		}
	}
}
