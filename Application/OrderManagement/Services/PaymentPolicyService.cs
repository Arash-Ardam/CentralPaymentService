using Application.OrderManagement.Dtos.GroupedOrder;
using Domain.Banking.Account;

namespace Application.OrderManagement.Services
{
	internal class PaymentPolicyService : IPaymentPolicyService
	{
		public (bool IsValid, string ErrorMessage) ValidateGroupPaymentRequest(Account targetAccount, int numberOfTransactions, long totalAmount)
		{
			if (targetAccount.PaymentSettings.Batch.MaxTransactionsCount < numberOfTransactions)
				return (false, "max transaction count hit");

			if (targetAccount.PaymentSettings.Batch.MaxDailyAmount < totalAmount)
				return (false, "max transaction amount hit");

			return (true, string.Empty);
		}
	}
}
