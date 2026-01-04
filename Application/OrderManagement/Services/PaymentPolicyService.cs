using Application.OrderManagement.Dtos.GroupedOrder;
using Domain.Banking.Account;

namespace Application.OrderManagement.Services
{
	internal class PaymentPolicyService : IPaymentPolicyService
	{
		public void ValidateGroupPaymentRequest(Account targetAccount, int numberOfTransactions, long totalAmount)
		{
			if (targetAccount.PaymentSettings.Batch.MaxTransactionsCount < numberOfTransactions)
				throw new ArgumentException($"max transaction count hit");

			if (targetAccount.PaymentSettings.Batch.MaxDailyAmount < totalAmount)
				throw new ArgumentException($"max transaction amount hit");
		}
	}
}
