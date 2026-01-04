using Application.OrderManagement.Ports.Abstractions;
using Application.OrderManagement.Ports.GroupedPaymentServices.Dtos;
using Domain.Banking.Bank.Enums;

namespace Application.OrderManagement.Ports.GroupedPaymentServices
{
	public interface IWithdrawalPaymentService
	{
		public BankCode BankCode { get; }
		Task<ProviderResponse<WithdrawalResponseDto>> SendRequestAsync(WithdrawalRequestDto request);

		Task<ProviderResponse<WithdrawalOrderInquiryResponseDto>> InquiryOrderAsync(WithdrawalOrderInquiryRequestDto request);

		Task<ProviderResponse<WithdrawalTransactionInquiryResponseDto>> InquiryTransactionAsync(WithdrawalTransactionInquiryRequestDto request);
	}
}
