using Application.Accounting.CustomerApp.Dtos;
using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.MultiTenancyServices
{
	internal class TenantContext : ITenantContext
	{
		private readonly AdminEfCoreDbContext _dbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TenantContext(AdminEfCoreDbContext dbContext, IHttpContextAccessor contextAccessor)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_httpContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
		}

		public string? TenantName { get; set; }
		public CustomerInfoDto? CustomerInfo { get; set; }

		public CustomerInfoDto? GetCurrentTenant()
		{
			return CustomerInfo;
		}

		public void SetTenant(string tenantName)
		{
			var customerInfo = (
								from customer in _dbContext.Customers.AsNoTracking()
								where customer.TenantName == tenantName
								select new CustomerInfoDto
								{
									FirstName = customer.Info.FirstName,
									LastName = customer.Info.LastName,
									Id = customer.Id,
									NationalCode = customer.Info.NationalCode,
									TenantName = customer.TenantName,
									IsEnable = customer.IsEnable,
									ConnectionString = customer.ConnectionString
								}).FirstOrDefault();

			if (customerInfo is null)
				throw new ArgumentException($"tenant: {tenantName} is invalid");

			CustomerInfo = customerInfo;
			TenantName = CustomerInfo.TenantName;
		}

		public void SetTenant()
		{
			var tenantName = _httpContextAccessor.HttpContext?
				.Request.Headers["X-Tenant"]
				.FirstOrDefault();

			if (tenantName != null || tenantName != TenantName)
			{
				var customerInfo = (
								from customer in _dbContext.Customers.AsNoTracking()
								where customer.TenantName == tenantName
								select new CustomerInfoDto
								{
									FirstName = customer.Info.FirstName,
									LastName = customer.Info.LastName,
									Id = customer.Id,
									NationalCode = customer.Info.NationalCode,
									TenantName = customer.TenantName,
									IsEnable = customer.IsEnable,
									ConnectionString = customer.ConnectionString
								}).FirstOrDefault();

				if (customerInfo is null)
					throw new ArgumentException($"tenant: {tenantName} is invalid");

				CustomerInfo = customerInfo;
				TenantName = CustomerInfo.TenantName;
			}
		}
	}
}
