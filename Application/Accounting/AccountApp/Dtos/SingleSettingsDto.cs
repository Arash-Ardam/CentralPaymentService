namespace Application.Accounting.AccountApp.Dtos
{
	public record SingleSettingsDto(
		Guid AccountId,
		string? TerminalId,
		string? MerchantId,
		string? username,
		string? password,
		DateTimeOffset? contractExpire);
}
