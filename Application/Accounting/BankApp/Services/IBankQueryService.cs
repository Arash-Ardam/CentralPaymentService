using Application.Accounting.BankApp.Dtos;
using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Services;

public interface IBankQueryService
{
	public Task<BankInfoDto?> GetAsync(Guid bankId);
	public Task<List<BankInfoDto>> GetAllAsync();
	Task<bool> IsExists(string name, BankCode bankCode);

}
