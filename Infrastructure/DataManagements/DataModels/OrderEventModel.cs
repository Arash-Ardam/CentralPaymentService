using Application.OrderManagement.Enums;
using Domain.Order.Enums;

namespace Infrastructure.DataManagements.DataModels
{
	internal class OrderEventModel
	{
		public Guid Id { get; set; }

		public string OrderId { get; set; }

		public OrderEventType EventType { get; set; }

		public PaymentType PaymentType { get; set; }

		public OrderStatus Status { get; set; }

		public string Payload { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public bool Processed { get; set; }

		public DateTimeOffset? ProcessedAt { get; set; }

		public int RetryCount { get; set; }

		public string? Error { get; set; }
	}
}
