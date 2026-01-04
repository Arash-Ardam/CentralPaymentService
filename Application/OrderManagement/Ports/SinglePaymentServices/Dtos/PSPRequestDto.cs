

namespace Application.OrderManagement.Ports.SinglePaymentServices.Dtos
{
	public class PSPRequestDto
	{
		public string SourceAccountNumber { get; set; }
		public string SourceAccountIban { get; set; }
		public string SourceFirstName { get; set; } = string.Empty;
		public string SourceLastName { get; set; } = string.Empty;

		public string DestinationAccountNumber { get; set; }
		public string DestinationFirstName { get; set; } = string.Empty;
		public string DestinationLastName { get; set; } = string.Empty;

		public long Amount { get; set; }
		public string Description { get; set; }
		public string PaymentId { get; internal set; } = string.Empty;

		public string TerminalId { get;  set; }= string.Empty;
		public string MerchantId { get;  set; }= string.Empty;
		public string Username { get;  set; }  = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
