namespace Domain.Customer.Services
{
	public interface ICustomerService
	{
		Task<bool> isCustomerExists(string tenantName);

	}
}
