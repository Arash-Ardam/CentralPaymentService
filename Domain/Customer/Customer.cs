using Domain.Customer.ValueObjects;

namespace Domain.Customer;

public class Customer
{
	public Guid Id { get; set; }
	public bool IsEnable { get; private set; } = true;
	public string TenantName { get; private set; } = string.Empty;
	public CustomerInformation? Info { get; private set; }

	public Customer(string tenantName)
	{
		TenantName = tenantName;
	}

	public void SetInformation(CustomerInformation info) => Info = info;

	public void ChangeStatus(bool status) => IsEnable = status;

	


}
