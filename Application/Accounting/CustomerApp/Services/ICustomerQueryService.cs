using Application.Accounting.CustomerApp.Dtos;
using Domain.Customer;

namespace Application.Accounting.CustomerApp.Services;

public interface ICustomerQueryService
{
	Task<CustomerInfoDto?> GetAsync(Guid id);

	Task<List<CustomerInfoDto>> GetAllAsync();

	Task<bool> IsExists(string tenantName);

}
