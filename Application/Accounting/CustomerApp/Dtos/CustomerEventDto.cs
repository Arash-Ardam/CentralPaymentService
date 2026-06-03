using Application.Accounting.CustomerApp.Enums;

namespace Application.Accounting.CustomerApp.Dtos
{
	public class CustomerEventDto
	{
		public string TenantName { get; set; }
		public string? Payload { get; set; }
		public CustomerEventType Type { get; set; }
		public string? ConnectionString { get; set; }
	}
}
