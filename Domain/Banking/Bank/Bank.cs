using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.ValueObjects;

namespace Domain.Banking.Bank;

public class Bank
{
	public Guid Id { get; private set; }

	public BankCode Code { get; private set; } = BankCode.None;
	public string Name { get; private set; } = string.Empty;
	public bool isEnable { get; private set; } = true;

	public List<ServiceTypes> ServiceTypes { get; set; } = new();

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

	public void RemoveServices()
	{
		ServiceTypes.RemoveAll(x => x != Enums.ServiceTypes.None);
	}

	public void ChangeStatus(bool status) => isEnable = status;

	public (bool HasService,string ErrMessage) EnsureHasGroupedService()
	{
		if(!isEnable)
			return (false, "Bank is disabled");

		if (!ServiceTypes.Contains(Enums.ServiceTypes.Grouped))
			return (false, "No Grouped payment service for account bank");

		return (true, string.Empty);
	}

	public (bool HasService,string Message) CheckSingleService()
	{
		if (!isEnable)
			return (false, "Bank is disabled");

		if (!ServiceTypes.Contains(Enums.ServiceTypes.Single))
			return (false, "No Single payment service exists for account bank");

		return (true, "Single payment service is available");
	}

}
