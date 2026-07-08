using Application.Accounting.CustomerApp.Dtos;
using Application.OrderManagement.Services;
using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Microsoft.AspNetCore.Http;

internal class TenantContext : ITenantContext
{
	private readonly ITenantRegistryService _tenantRegistryService;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CustomerInfoDto? Current { get; private set; }

	public TenantContext(
		IHttpContextAccessor httpContextAccessor,
		ITenantRegistryService tenantRegistryService)
	{
		_httpContextAccessor = httpContextAccessor;
		_tenantRegistryService = tenantRegistryService;
	}

	public void SetTenantByUser()
	{
		var tenantName = _httpContextAccessor.HttpContext?
			.User.Claims.FirstOrDefault(claim => claim.Type == "X-Tenant")
			.Value;

		if (string.IsNullOrWhiteSpace(tenantName))
			throw new ArgumentException("X-Tenant header is missing");

		Current = LoadTenant(tenantName);
	}

	public void SetTenantByAdmin(string tenantName)
	{
		if (string.IsNullOrWhiteSpace(tenantName))
			throw new ArgumentException("tenantName is required");

		Current = LoadTenant(tenantName);
	}

	private CustomerInfoDto LoadTenant(string tenantName)
	{
		var customer = _tenantRegistryService
			.Find(tenantName);

		if (customer is null)
			throw new ArgumentException($"Tenant '{tenantName}' not found");

		return new CustomerInfoDto
		{
			Id = customer.Id,
			TenantName = customer.Name,
			ConnectionString = customer.ConnectionString,
			IsEnable = customer.IsEnable
		};
	}
}
