using Application.Accounting.CustomerApp.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class CustomerEventModel
	{
		public int Id { get; set; }
		public string TenantName { get; set; }
		public CustomerEventType Type { get; set; }
		public string? ConnectionString { get; set; }
		public bool IsProccessed { get; set; } = false;
		public string Payload { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? ProccessedAt { get; set; }
		public string? Error { get; set; }
		public int RetryCount { get; set; } = 0;
	}
}
