using Application.OrderManagement.Services;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.MultiTenancyServices.TenantResolver;
using Microsoft.Extensions.Options;

internal class TenantResolver : ITenantResolver
{
	private readonly ITenantContext _tenantContext;
	private readonly ORMToolsOptions _options;

	public TenantResolver(
		ITenantContext tenantContext,
		IOptions<ORMToolsOptions> options)
	{
		_tenantContext = tenantContext;
		_options = options.Value;
	}

	public string Resolve()
	{
		if (_tenantContext.Current is not null)
		{
			return string.IsNullOrWhiteSpace(_tenantContext.Current.ConnectionString)
				? string.Format(_options.EfCore.TenantConnectionString,
					_tenantContext.Current.TenantName)
				: _tenantContext.Current.ConnectionString;
		}
		return string.Empty;
	}
}
