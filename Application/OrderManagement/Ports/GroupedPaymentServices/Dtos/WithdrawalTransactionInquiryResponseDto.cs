using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.GroupedPaymentServices.Dtos
{
	public class WithdrawalTransactionInquiryResponseDto
	{
		public GroupedTransactionStatus Status {  get; set; }
		public string PaymentId { get; set; } = string.Empty;
		public string TrackingCode { get; set; } = string.Empty;
		public string ProviderMessage { get; set; } = string.Empty;
	}
}
