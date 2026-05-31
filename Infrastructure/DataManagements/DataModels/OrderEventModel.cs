using Domain.Order.Enums;
using Infrastructure.DataManagements.Abstractions.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class OrderEventModel
	{
		public Guid Id { get; set; }

		public string TenantName { get;  set; }

		public string OrderId { get;  set; }

		public OrderEventType EventType { get;  set; }

		public PaymentType PaymentType { get;  set; }

		public string Payload { get;  set; }

		public DateTimeOffset CreatedAt { get;  set; }

		public bool Processed { get;  set; }

		public DateTimeOffset? ProcessedAt { get;  set; }

		public string? Error { get;  set; }
	}
}
