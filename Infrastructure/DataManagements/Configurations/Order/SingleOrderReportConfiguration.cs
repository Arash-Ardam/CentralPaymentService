using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManagements.Configurations.Order
{
	internal class SingleOrderReportConfiguration : IEntityTypeConfiguration<SingleOrderReportModel>
	{
		public void Configure(EntityTypeBuilder<SingleOrderReportModel> builder)
		{
			/*
			 Duplicate Execution
	فرض کن سرویس ریستارت شود.		
	بین:
	AddReport و Processed = true کرش کند.

	در اجرای بعدی دوباره همان Event پردازش می‌شود.	

	برای همین پیشنهاد می‌کنم روی جدول گزارش:

	OrderId

Unique Index داشته باشی.
مثلاً:
builder.HasIndex(x => x.OrderId)
       .IsUnique();
در این صورت اگر Event دوبار اجرا شد:

idempotent

می‌شود.
			 */
			builder.HasIndex(x => x.OrderId)
				.IsUnique();

		}
	}
}
