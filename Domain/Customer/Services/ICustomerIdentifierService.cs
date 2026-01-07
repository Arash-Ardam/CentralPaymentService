namespace Domain.Customer.Services
{
	public interface ICustomerIdentifierService
	{
		Task<bool> isCustomerExists(string tenantName);

	}
}
