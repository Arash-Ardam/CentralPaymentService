using Application.OrderManagement.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataManagements.MultiTenancyServices.TenantResolver
{
	internal class TenantResolver : ITenantResolver
	{
		private readonly ORMToolsOptions _oRMOptions;
		private readonly ITenantContext _tenantContext;

		public TenantResolver(ITenantContext tenantContext, IOptions<ORMToolsOptions> options)
		{
			_oRMOptions = options.Value;
			_tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
		}


		public string Resolve()
		{
			_tenantContext.SetTenant();

			if (_tenantContext.CustomerInfo is null)
				throw new ArgumentException("X-Tenant header null or invalid");
			
			return string.IsNullOrWhiteSpace(_tenantContext.CustomerInfo.ConnectionString) 
				? string.Format(_oRMOptions.EfCore.TenantConnectionString, _tenantContext.CustomerInfo.TenantName) 
				: _tenantContext.CustomerInfo.ConnectionString;
		}
	}
}
