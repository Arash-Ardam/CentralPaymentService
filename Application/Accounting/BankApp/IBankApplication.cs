using Application.Abstractions;
using Application.Accounting.BankApp.Dtos;
using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp
{
	public interface IBankApplication
	{
		Task<ApplicationResponse<Guid>> CreateAsync(CreateBankDto bankDto);
		Task<ApplicationResponse> AssignPaymentServices(Guid bankId, List<ServiceTypes> services);
		Task<ApplicationResponse> ChangeStatusAsync(Guid bankId, bool status);

		Task<ApplicationResponse<BankInfoDto>> GetAsync(Guid bankId);
		
	}
}
