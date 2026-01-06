using Domain.Order.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class OrderDataConfiguration : IEntityTypeConfiguration<Domain.Order.Order>
	{
		public void Configure(EntityTypeBuilder<Domain.Order.Order> builder)
		{
			builder.ToTable("Orders");

			builder.HasKey(x => x.Id);

			builder.OwnsOne(
				x => x.Specifics,
				nav =>
				{
					nav.Property(spec => spec.NumberOfTransactions)
					.HasDefaultValue(0);
				});

			builder
				.HasOne(x => x.SingleTransaction)
				.WithOne()
				.HasForeignKey<SingleTransaction>(x => x.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasMany(x => x.GroupedTransactions)
				.WithOne()
				.HasForeignKey(x => x.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
