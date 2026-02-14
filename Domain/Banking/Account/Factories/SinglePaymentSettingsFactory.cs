using Domain.Banking.Account.ValueObjects;

namespace Domain.Banking.Account.Factories;

public class SinglePaymentSettingsFactory
{
	private DateTimeOffset ContractExpire = DateTimeOffset.Now.AddMonths(6);
	private string TerminalId = string.Empty;
	private string MerchantId  = string.Empty;
	private string Username  = string.Empty;
	private string Password  = string.Empty;


	internal SinglePaymentSettingsFactory() { }

	public  SinglePaymentSettingsFactory WithTerminalId(string value)
	{
		TerminalId = value;
		return this;
	}

	public SinglePaymentSettingsFactory WithMerchantId(string value)
	{
		MerchantId = value;
		return this;
	}

	public SinglePaymentSettingsFactory WithUsername(string value)
	{
		Username = value;
		return this;
	}

	public SinglePaymentSettingsFactory WithPassword(string value)
	{
		Password = value;
		return this;
	}

	public SinglePaymentSettingsFactory WithExpire(DateTimeOffset value)
	{
		ContractExpire = value;
		return this;
	}


	public SingleSettings Build()
	{
		return new SingleSettings(
			TerminalId,
			MerchantId,
			Username,
			Password,
			ContractExpire
			);
	}
}
