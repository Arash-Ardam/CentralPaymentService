using Application.OrderManagement.Enums;
using Domain.Order.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class OutboxMessageModel
	{
		public Guid Id { get; set; }

		public string OutboxId { get; set; }

		public OutBoxType Type { get; set; }

		public OutboxBehaviorType BehaviorType { get; set; }

		public string TenantName { get; set; }

		public Guid TenantId { get; set; }

		public string Payload { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public bool Processed { get; set; }

		public DateTimeOffset? ProcessedAt { get; set; }

		public int RetryCount { get; set; } = 0;

		public string? Error { get; set; }
	}
}
