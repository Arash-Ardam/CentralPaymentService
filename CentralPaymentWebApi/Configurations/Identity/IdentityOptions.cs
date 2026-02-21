namespace CentralPaymentWebApi.Configurations.Identity
{
	public class IdentityOptions
	{
		public string Authority { get; set; }
		public string Audience { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string ValidIssuer { get; set; }
	}
}
