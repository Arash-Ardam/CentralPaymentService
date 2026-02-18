using Application.Accounting.CustomerApp.Dtos;

namespace Application.OrderManagement.Services
{
	public interface ITenantContext
	{
		CustomerInfoDto? Current { get; }
		void SetTenantByUser();
		void SetTenantByAdmin(string tenantName);
	}

}
