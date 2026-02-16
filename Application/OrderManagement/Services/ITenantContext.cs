using Application.Accounting.CustomerApp.Dtos;

namespace Application.OrderManagement.Services
{
	public interface ITenantContext
	{
		string? TenantName { get; }
		public CustomerInfoDto? CustomerInfo { get; set; }

		void SetTenant(string tenantName);
		void SetTenant();
		CustomerInfoDto? GetCurrentTenant();
	}
}
