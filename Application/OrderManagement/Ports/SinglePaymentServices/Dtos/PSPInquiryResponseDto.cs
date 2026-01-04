using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.SinglePaymentServices.Dtos
{
	public class PSPInquiryResponseDto
	{
		public string TrackingCode { get; set; } = string.Empty;
		public OrderStatus Status { get; set; }

	}
}
