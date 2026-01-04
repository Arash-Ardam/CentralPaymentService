using Application.OrderManagement.Ports.Abstractions;
using Application.OrderManagement.Ports.SinglePaymentServices.Dtos;
using Domain.Banking.Bank.Enums;

namespace Application.OrderManagement.Ports.SinglePaymentServices
{
	public interface IPSPPaymentService
	{
		BankCode BankCode { get; }

		Task<ProviderResponse<PSPResponseDto>> SendRequestAsync(PSPRequestDto request);

		Task<ProviderResponse<PSPInquiryResponseDto>> InquiryAsync(PSPInquiryRequestDto inquiryRequestDto);
	}
}
