using Infrastructure.DataManagements.Abstractions.Dtos;

namespace Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry
{
	internal interface ITenantRegistryService
	{
		Task RefreshAsync();
		IReadOnlyList<TenantInfoDto> GetAll();

		TenantInfoDto? Find(Guid id);
	}
}
