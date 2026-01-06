using Domain.Order.Entities;
using Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class GroupedTransactionConfiguration : IEntityTypeConfiguration<GroupedTransaction>
	{
		public void Configure(EntityTypeBuilder<GroupedTransaction> builder)
		{
			builder.ToTable("GroupedTransactions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Status)
				.HasDefaultValue(GroupedTransactionStatus.None);

			builder.OwnsOne(
				x => x.Specs,
				nav =>
				{
					nav.Property(spec => spec.Amount)
					.HasDefaultValue(0);

					nav.Property(spec => spec.TransactionType)
					.HasDefaultValue(TransactionType.None);
				});
		}
	}
}
