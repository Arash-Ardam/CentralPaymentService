namespace Domain.Banking.Account.Services
{
	public interface IAccountIdentifierService
	{
		Task<bool> IsExists(string accountNubmer, string accountIban);
	}
}
