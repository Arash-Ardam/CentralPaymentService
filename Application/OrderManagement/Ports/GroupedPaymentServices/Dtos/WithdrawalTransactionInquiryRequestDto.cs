namespace Application.OrderManagement.Ports.GroupedPaymentServices.Dtos
{
	public class WithdrawalTransactionInquiryRequestDto
	{
		public string PaymentId { get; set; } = string.Empty;
		public string TrackingId { get; set; } = string.Empty;
	}
}
