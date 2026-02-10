namespace CentralPaymentWebApi.Dtos.CustomerApi
{
	public class ChangeCustomerStatusDto
	{
		public Guid CustomerId { get; set; }
		public bool Status { get; set; }
	}
}
