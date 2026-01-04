namespace Application.Accounting.AccountApp.Dtos
{
	public record BatchSettingsDto(
		Guid AccountId,
		int MaxTransactionsCount,
		long MaxDailyAmount,
		long MinSatnaAmount,
		DateTimeOffset? ContractExpire
		);
	
		
	
}
