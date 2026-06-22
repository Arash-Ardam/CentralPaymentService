using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class GroupedOrderReportConfiguration : IEntityTypeConfiguration<GroupedOrderReportModel>
	{
		public void Configure(EntityTypeBuilder<GroupedOrderReportModel> builder)
		{
			builder.HasIndex(x => x.OrderId)
				.IsUnique();


			builder
				.HasMany(x => x.Transactions)
				.WithOne()
				.HasForeignKey(x => x.WithdrawalId);
		}
	}
}
