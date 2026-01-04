using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Dtos
{
	public record BatchSettingsDto(Guid BankId,long? maxTransactionCount, long? maxDailyAmount, DateTimeOffset? contractExpire);

}
