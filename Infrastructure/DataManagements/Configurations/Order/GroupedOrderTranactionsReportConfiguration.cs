using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class GroupedOrderTranactionsReportConfiguration : IEntityTypeConfiguration<GroupedOrderTransactionReportModel>
	{
		public void Configure(EntityTypeBuilder<GroupedOrderTransactionReportModel> builder)
		{
			builder.HasIndex(x => x.OrderId).IsUnique();
		}
	}
}
