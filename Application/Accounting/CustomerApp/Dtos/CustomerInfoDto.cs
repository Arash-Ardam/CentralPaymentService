namespace Application.Accounting.CustomerApp.Dtos
{
	public class CustomerInfoDto()
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string NationalCode { get; set; }
		public string TenantName { get; set; }
		public bool IsEnable { get; set; }
	}
}
	