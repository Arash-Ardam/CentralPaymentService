using Application.Accounting.CustomerApp.Dtos;
using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

internal class TenantContext : ITenantContext
{
	private readonly AdminEfCoreDbContext _dbContext;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CustomerInfoDto? Current { get; private set; }

	public TenantContext(
		AdminEfCoreDbContext dbContext,
		IHttpContextAccessor httpContextAccessor)
	{
		_dbContext = dbContext;
		_httpContextAccessor = httpContextAccessor;
	}

	public void SetTenantByUser()
	{
		var tenantName = _httpContextAccessor.HttpContext?
			.Request.Headers["X-Tenant"]
			.FirstOrDefault();

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
		var customer = _dbContext.Customers
			.AsNoTracking()
			.Where(c => c.TenantName == tenantName)
			.Select(c => new CustomerInfoDto
			{
				Id = c.Id,
				TenantName = c.TenantName,
				ConnectionString = c.ConnectionString,
				IsEnable = c.IsEnable
			})
			.FirstOrDefault();

		if (customer is null)
			throw new ArgumentException($"Tenant '{tenantName}' not found");

		return customer;
	}
}
