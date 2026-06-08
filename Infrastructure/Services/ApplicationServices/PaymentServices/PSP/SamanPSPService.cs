using Application.OrderManagement.Ports.Abstractions;
using Application.OrderManagement.Ports.SinglePaymentServices;
using Application.OrderManagement.Ports.SinglePaymentServices.Dtos;
using Domain.Banking.Bank.Enums;
using Domain.Order.Enums;

namespace Infrastructure.Services.ApplicationServices.PaymentServices.PSP
{
	internal sealed class SamanPSPService : IPSPPaymentService
	{
		public BankCode BankCode => BankCode.Saman;

		public Task<ProviderResponse<PSPInquiryResponseDto>> InquiryAsync(PSPInquiryRequestDto inquiryRequestDto)
		{
			return Task.FromResult(new ProviderResponse<PSPInquiryResponseDto>
			{
				StatusCode = 200,
				IsSuccess = true,
				Data = new PSPInquiryResponseDto
				{
					Status = OrderStatus.Done
				}
			});
		}

		public Task<ProviderResponse<PSPResponseDto>> SendRequestAsync(PSPRequestDto request)
		{
			var response = new ProviderResponse<PSPResponseDto>();
			if (request.Amount > 10000000)
			{
				response.StatusCode = 400;
				response.IsSuccess = false;
				response.Message = "Amount exceeds the limit for Saman PSP.";
			}

			else
			{
				response.StatusCode = 200;
				response.IsSuccess = true;
				response.Data = new PSPResponseDto
				{
					TrackingCode = $"{request.SourceAccountNumber}_{request.DestinationAccountNumber}_0",
					Status = OrderStatus.Done
				};
			}
			
			return Task.FromResult(response);
		}
	}
}
