namespace CentralPaymentWebApi.Dtos.AccountApi
{
	public class ChangeAccountStatusDto
	{
		public Guid AccountId { get; set; }
		public bool Status { get; set; }
	}
}
