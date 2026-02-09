namespace CentralPaymentWebApi.Dtos.CustomerApi
{
	public class ChangeStatusDto
	{
		public Guid CustomerId { get; set; }
		public bool Status { get; set; }
	}
}
