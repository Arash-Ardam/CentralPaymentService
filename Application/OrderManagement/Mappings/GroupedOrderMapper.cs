using Application.OrderManagement.Ports.GroupedPaymentServices.Dtos;
using Application.OrderManagement.Ports.SinglePaymentServices.Dtos;
using Domain.Banking.Account;
using Domain.Customer;
using Domain.Order;

namespace Application.OrderManagement.Mappings
{
	public class GroupedOrderMapper
	{
		public static WithdrawalRequestDto MapToRequest(Customer customer,Account account,Order order)
		{
			return new WithdrawalRequestDto()
			{
				SourceAccountNumber = account.AccountNumber,
				SourceAccountIban = account.Iban,
				RequestPaymentId = order.OrderId,
				SourceFirstName = customer.Info.FirstName,
				SourceLastName = customer.Info.LastName,
				Transactions = order.GroupedTransactions.Select(trx => new WithdrawalRequestTransactions()
				{
					AccountNumber = trx.Specs.AccountNumber,
					Iban = trx.Specs.Iban,
					NationalId = trx.Specs.NationalId,
					FirstName = trx.Specs.FirstName,
					LastName = trx.Specs.LastName,
					Amount	= trx.Specs.Amount,
					Description	= trx.Specs.Description,
					PaymentId = trx.Specs.PaymentId,
					TransactionType = trx.Specs.TransactionType
				}).ToList()
			};
		}

		public static WithdrawalOrderInquiryRequestDto MapToOrderInquiryRequest(string trackingCode)
		{
			return new WithdrawalOrderInquiryRequestDto()
			{
				TrackingCode = trackingCode
			};
		}

		public static WithdrawalTransactionInquiryRequestDto MapToTransactionInquiryRequest(string trackingCode,string paymentId)
		{
			return new WithdrawalTransactionInquiryRequestDto()
			{
				TrackingId = trackingCode,
				PaymentId = paymentId
			};
		}


	}
}
