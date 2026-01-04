using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.SinglePaymentServices.Dtos
{
	public class PSPInquiryRequestDto
	{
		public string TrackingCode { get; set; } = string.Empty;
	}
}
