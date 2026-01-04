using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.SinglePaymentServices.Dtos
{
	public class PSPResponseDto
	{
		public string TrackingCode { get; set; } = string.Empty;
		public OrderStatus Status { get; set; }
	}
}
