using Domain.Order.Enums;

namespace Application.OrderManagement.Ports.GroupedPaymentServices.Dtos
{
	public record WithdrawalResponseDto
	{
		public string TrackingCode { get; set; } = string.Empty;
		public OrderStatus OrderStatus { get; set; }

		public List<WithdrawalTransactionResponseDto> TrasactionsResponse { get; set; } = new List<WithdrawalTransactionResponseDto>();


	}

	public record WithdrawalTransactionResponseDto
	{
		public string PaymentId { get; set; }
		public string TrackingCode { get; set; }
		public string ProviderMessage { get; set; }
		public GroupedTransactionStatus Status { get; set; }
	}
}
