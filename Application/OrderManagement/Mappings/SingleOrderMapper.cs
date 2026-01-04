using Application.OrderManagement.Ports.SinglePaymentServices.Dtos;
using Domain.Banking.Account;
using Domain.Customer;
using Domain.Order;

namespace Application.OrderManagement.Mappings
{
	public class SingleOrderMapper
	{
		public static PSPRequestDto MapToRequest(Order order,Account account,Customer customer)
		{
			return new PSPRequestDto() 
			{
				SourceAccountNumber = account.AccountNumber,
				SourceAccountIban = account.Iban,
				SourceFirstName = customer.Info.FirstName,
				SourceLastName = customer.Info.LastName,

				DestinationAccountNumber = order.SingleTransaction.Specs.AccountNumber,
				DestinationFirstName = order.SingleTransaction.Specs.FirstName,
				DestinationLastName = order.SingleTransaction.Specs.LastName,

				Description = order.SingleTransaction.Specs.Description,
				PaymentId = order.SingleTransaction.Specs.PaymentId,
				Amount = order.SingleTransaction.Specs.Amount,

				TerminalId = account.PaymentSettings.Single.TerminalId,
				MerchantId = account.PaymentSettings.Single.MerchantId,
				Username = account.PaymentSettings.Single.Username,
				Password = account.PaymentSettings.Single.Password
			};
		}

		public static PSPInquiryRequestDto MapToInquiryRequest(string trackingCode)
		{
			return new PSPInquiryRequestDto()
			{
				TrackingCode = trackingCode
			};
		}

	}
}
