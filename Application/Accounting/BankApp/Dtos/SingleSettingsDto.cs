namespace Application.Accounting.BankApp.Dtos
{
	public record SingleSettingsDto(Guid BankId, string? TerminalId,string? MerchantId,DateTime? ContractExpire);
}
