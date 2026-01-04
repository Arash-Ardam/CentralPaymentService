using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.GroupedPaymentServices.Dtos
{
	public class WithdrawalOrderInquiryResponseDto
	{
		public OrderStatus status { get; set; }
		public string ProviderMessage { get; set; } = string.Empty;
		public string TrackingCode { get; set; } = string.Empty;

		public List<WithdrawalTransactionInquiryResponseDto> Transactions { get; set; } = new();

	}


}
