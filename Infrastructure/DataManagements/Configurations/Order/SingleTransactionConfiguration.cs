using Domain.Order.Entities;
using Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class SingleTransactionConfiguration : IEntityTypeConfiguration<SingleTransaction>
	{
		public void Configure(EntityTypeBuilder<SingleTransaction> builder)
		{
			builder.ToTable("SingleTransactions");

			builder.HasKey(t => t.Id);

			builder.OwnsOne(
				x => x.Specs,
				nav =>
				{
					nav.Property(spec => spec.Amount)
					.HasDefaultValue(0);

					nav.Property(spec => spec.TransactionType)
					.HasDefaultValue(TransactionType.PSP);
				});
		}
	}
}
