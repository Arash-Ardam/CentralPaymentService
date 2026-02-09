using Application.Abstractions;
using Application.Accounting.CustomerApp.Dtos;

namespace Application.Accounting.CustomerApp
{
	public interface ICustomerApplication
	{
		Task<ApplicationResponse<Guid>> CreateAsync(CreateCustomerDto createCustomerDto);
		Task<ApplicationResponse<Guid>> SetCustomerSettings(InformationDto informationDto);
		Task<ApplicationResponse<Guid>> ChangeStatus(Guid customerId, bool status);
		Task<ApplicationResponse<CustomerInfoDto>> GetAsync(Guid customerId);
		Task<ApplicationResponse<List<CustomerInfoDto>>> GetAllAsync();
	}
}
