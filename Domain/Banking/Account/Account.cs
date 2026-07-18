using Domain.Banking.Account.ValueObjects;

namespace Domain.Banking.Account;

public class Account
{
	public Guid Id { get; private set; }

	public string AccountNumber { get; private set; }
	public string Iban { get; private set; }
	public DateTimeOffset? ExpirationDate { get; private set; } 
	public bool IsEnable { get; private set; } = true;

	public Guid CustomerId { get; private set; }
	public Guid BankId { get; private set; }

	public PaymentSettings PaymentSettings { get; private set; } = new PaymentSettings();

	public Account(string accountNumber, string iban, DateTimeOffset expireDate)
	{
		AccountNumber = accountNumber;
		Iban = iban;
		ExpirationDate = (expireDate == default) ? DateTimeOffset.UtcNow.AddMonths(6) : expireDate;
	}

	// for ORM bindings
	private Account() { }

	public void AssignToCustomer(Guid id) => CustomerId = id;
	public void AssignToBank(Guid id) => BankId = id;
	public void ChangeStatus(bool status) => IsEnable = status;

	public void EnsureGroupedServiceAvailable()
	{
		if (!IsEnable)
			throw new ArgumentException("account is disabled");

		if (PaymentSettings.Batch is null || !PaymentSettings.Batch.IsEnable)
			throw new ArgumentException("target account doesn't have batch payment settings or is disabled");

		if (PaymentSettings.Batch.ContractExpire < DateTimeOffset.Now)
			throw new ArgumentException("The service is expired");
	}

	public (bool IsAvailable, string Message) EnsureSingleServiceAvailable()
	{
		if (PaymentSettings.Single is null)
			return (false, "Bank Single payment service is not available for target account");

		if (!PaymentSettings.Single.IsEnable)
			return (false, "Bank Single payment service is not enable for target account");

		if (PaymentSettings.Single.ContractExpire < DateTimeOffset.Now)
			return (false, "Bank Single payment service is expired for target account");

		return (true, "Bank Single payment service is available for target account");
	}

}
