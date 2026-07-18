using Infrastructure.DataManagements.DataModels;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DataManagements.Configurations.Idempotency
{
	internal sealed class IdempotencyDataConfiguration : IEntityTypeConfiguration<IdempotencyModel>
	{
		public void Configure(EntityTypeBuilder<IdempotencyModel> builder)
		{
			builder.ToTable("IdempotencyRequests");

			builder.HasKey(x => x.Id);

			builder
				.Property(x => x.Key)
				.IsRequired();

			builder
				.Property(x => x.RequestBody)
				.HasConversion(new RequestBodyConvertor());

			builder
				.Property(x => x.ResponseBody)
				.IsRequired(false);

			builder
				.Property(x => x.StatusCode)
				.IsRequired(false);

			builder
				.Property(x => x.CreatedAt)
				.HasDefaultValue(DateTimeOffset.UtcNow);
				
		}
	}


	internal sealed class RequestBodyConvertor : ValueConverter<string, string>
	{
		public RequestBodyConvertor() : base(
			v => ConvertToDatabase(v),
			v => ConvertFromDatabase(v))
		{
		}
		private static string ConvertToDatabase(string value) => HashConvertor.ConvertToHash(value);
		private static string ConvertFromDatabase(string value) => HashConvertor.ConvertFromHash(value);
	}

}
