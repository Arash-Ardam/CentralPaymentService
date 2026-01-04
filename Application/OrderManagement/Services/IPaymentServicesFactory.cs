using Application.OrderManagement.Ports.GroupedPaymentServices;
using Application.OrderManagement.Ports.SinglePaymentServices;
using Domain.Banking.Bank.Enums;

namespace Application.OrderManagement.Services
{
	public interface IPaymentServicesFactory
	{
		IPSPPaymentService? GetPSPPaymentService(BankCode bankCode);
		IWithdrawalPaymentService? GetWithdrawalPaymentService(BankCode bankCode);
	}
}
