using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Account
{
	internal class AccountDataConfiguration : IEntityTypeConfiguration<Domain.Banking.Account.Account>
	{
		public void Configure(EntityTypeBuilder<Domain.Banking.Account.Account> builder)
		{
			builder.ToTable("Accounts");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.AccountNumber)
				.HasMaxLength(30);

			builder.Property(x => x.Iban)
				.HasMaxLength(26);

			builder.Property(x => x.IsEnable)
				.HasDefaultValue(true);

			builder.OwnsOne(
			x => x.PaymentSettings,
			ps =>
			{
				ps.WithOwner();

				// ---------- BatchSettings ----------
				ps.OwnsOne(
					x => x.Batch,
					batch =>
					{
						batch.Property(x => x.IsEnable)
							 .HasColumnName("Batch_IsEnable");

						batch.Property(x => x.ContractExpire)
							 .HasColumnName("Batch_ContractExpire");

						batch.Property(x => x.MinSatnaAmount)
							 .HasColumnName("Batch_MinSatnaAmount");
					});

				// ---------- SingleSettings ----------
				ps.OwnsOne(
					x => x.Single,
					single =>
					{
						single.Property(x => x.IsEnable)
							  .HasColumnName("Single_IsEnable");

						single.Property(x => x.ContractExpire)
							  .HasColumnName("Single_ContractExpire");
					});
			});
		}
	}
}
