using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.OutboxMessage
{
	internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessageModel>
	{
		public void Configure(EntityTypeBuilder<OutboxMessageModel> builder)
		{
			builder.Property(x => x.TenantId)
				.IsRequired();

			builder.Property(x => x.Payload)
				.IsRequired(false);

			builder.Property(x => x.RetryCount)
				.HasDefaultValue(0);
		}
	}
}
