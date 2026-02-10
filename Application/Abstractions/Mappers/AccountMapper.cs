using Application.Accounting.AccountApp.Dtos;
using AutoMapper;
using Domain.Banking.Account;
using Domain.Banking.Account.ValueObjects;

namespace Application.Abstractions.Mappers
{
	internal class AccountMapper : Profile
	{
		public AccountMapper()
		{
			CreateMap<SingleSettings, SingleSettingsInfoDto>()
				.ForMember(mem => mem.Status, opt => opt.MapFrom(src => src.IsEnable))
				.ForMember(mem => mem.ExpireDate, opt => opt.MapFrom(src => src.ContractExpire));

			CreateMap<BatchSettings, BatchSettingsInfoDto>()
				.ForMember(mem => mem.Status, opt => opt.MapFrom(src => src.IsEnable))
				.ForMember(mem => mem.ExpireDate, opt => opt.MapFrom(src => src.ContractExpire));

			CreateMap<Account, AccountInfoDto>()
				.ForMember(mem => mem.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
				.ForMember(mem => mem.Iban, opt => opt.MapFrom(src => src.Iban))
				.ForMember(mem => mem.Status, opt => opt.MapFrom(src => src.IsEnable))
				.ForMember(mem => mem.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
				.ForMember(mem => mem.SingleService, opt => opt.MapFrom(src => src.PaymentSettings.Single))
				.ForMember(mem => mem.BatchService, opt => opt.MapFrom(src => src.PaymentSettings.Batch));
		}
	}
}
