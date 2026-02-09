using Application.Accounting.BankApp.Dtos;
using AutoMapper;
using Domain.Banking.Bank;

namespace Application.Abstractions.Mappers
{
	internal class BankMapper : Profile
	{
		public BankMapper()
		{
			CreateMap<Bank, BankInfoDto>()
				.ForMember(mem => mem.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(mem => mem.Title, opt => opt.MapFrom(src => src.Name))
				.ForMember(mem => mem.Status, opt => opt.MapFrom(src => src.isEnable));
		}
	}
}
