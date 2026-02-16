namespace Infrastructure.DataManagements.MultiTenancyServices.TenantResolver
{
	internal interface ITenantResolver
	{
		string Resolve();
	}
}
