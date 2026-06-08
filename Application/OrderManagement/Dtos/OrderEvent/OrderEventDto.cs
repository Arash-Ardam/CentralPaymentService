using Application.OrderManagement.Enums;
using Domain.Order.Enums;

namespace Application.OrderManagement.Dtos.OrderEvent
{
	public sealed class OrderEventDto
	{
		public string OrderId { get; set; }

		public OrderEventType EventType { get; set; }

		public PaymentType PaymentType { get; set; }

		public OrderStatus Status { get; set; }

		public string Payload { get; set; }

		public DateTimeOffset CreatedAt { get; private set; }

		public bool Processed { get; private set; } = false;

		public DateTimeOffset? ProcessedAt { get; private set; }

		public string? Error { get; private set; }
	}
}
