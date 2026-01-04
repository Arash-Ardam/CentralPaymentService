using Application.OrderManagement.Ports.GroupedPaymentServices;
using Application.OrderManagement.Ports.SinglePaymentServices;
using Domain.Banking.Bank.Enums;

namespace Application.OrderManagement.Services
{
	internal class PaymentServicesFactory : IPaymentServicesFactory
	{
		private readonly IEnumerable<IPSPPaymentService> _pspServices;
		private readonly IEnumerable<IWithdrawalPaymentService> _withdrawalServices;

		public PaymentServicesFactory(IEnumerable<IPSPPaymentService> pspServices,IEnumerable<IWithdrawalPaymentService> withdrawalServices)
		{
			_pspServices = pspServices;
			_withdrawalServices = withdrawalServices;
		}

		public IPSPPaymentService? GetPSPPaymentService(BankCode bankCode)
		{
			return _pspServices.FirstOrDefault(s => s.BankCode == bankCode);
		}

		public IWithdrawalPaymentService? GetWithdrawalPaymentService(BankCode bankCode)
		{
			return _withdrawalServices.FirstOrDefault(s => s.BankCode == bankCode);
		}
	}
}
