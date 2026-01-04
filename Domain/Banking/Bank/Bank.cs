using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.ValueObjects;

namespace Domain.Banking.Bank;

public class Bank
{
	public Guid Id { get; private set; }

	public BankCode Code { get; private set; } = BankCode.None;
	public string Name { get; private set; } = string.Empty;
	public bool isEnable { get; private set; } = true;

	public List<ServiceTypes> ServiceTypes { get; set; }

	public Bank(string name, BankCode code)
	{
		Name = name;
		Code = code;
	}

	public void AddService(ServiceTypes service)
	{
		if(!ServiceTypes.Contains(service) && service != Enums.ServiceTypes.None) 
			ServiceTypes.Add(service);
	}

	public void RemoveService(ServiceTypes service)
	{
		if (ServiceTypes.Contains(service) && service != Enums.ServiceTypes.None)
			ServiceTypes.Remove(service);
	}

	public void ChangeStatus(bool status) => isEnable = status;

	public void EnsureHasGroupedService()
	{
		if(!isEnable)
			throw new ArgumentException("Bank is disabled");

		if (!ServiceTypes.Contains(Enums.ServiceTypes.Grouped))
			throw new ArgumentException("No Grouped payment service for account bank");
	}

	public void EnsureHasSingleService()
	{
		if (!isEnable)
			throw new ArgumentException("Bank is disabled");

		if (!ServiceTypes.Contains(Enums.ServiceTypes.Single))
			throw new ArgumentException("No Single payment service exists for account bank");
	}

}
