namespace Domain.Banking.Account.ValueObjects
{
	public class SingleSettings
	{
		public bool IsEnable { get; set; } = true;

		public DateTimeOffset ContractExpire { get; private set; } 
		public string TerminalId { get; private set; } 
		public string MerchantId { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		internal SingleSettings(string terminalId,string merchantId,string username,string password,DateTimeOffset contractExpire)
		{
			TerminalId = terminalId;
			MerchantId = merchantId;
			Username = username;
			Password = password;
			ContractExpire = contractExpire;
		}

		public void Enable() => IsEnable = true;
		public void Disable() => IsEnable = false;

	}
}
