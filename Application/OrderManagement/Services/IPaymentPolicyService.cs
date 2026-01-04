using Application.OrderManagement.Dtos.GroupedOrder;
using Domain.Banking.Account;

namespace Application.OrderManagement.Services
{
	public interface IPaymentPolicyService
	{
		void ValidateGroupPaymentRequest(Account targetAccount,int numberOfTransactions,long totalAmount);
	}
}
