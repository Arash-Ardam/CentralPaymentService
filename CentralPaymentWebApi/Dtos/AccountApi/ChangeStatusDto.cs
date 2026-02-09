namespace CentralPaymentWebApi.Dtos.AccountApi
{
	public class ChangeStatusDto
	{
		public Guid AccountId { get; set; }
		public bool Status { get; set; }
	}
}
