using Application.Accounting.CustomerApp.Dtos;
using AutoMapper;
using Domain.Customer;

namespace Application.Abstractions.Mappers
{
	internal class CustomerMapper : Profile 
	{
		public CustomerMapper()
		{
			CreateMap<Customer, CustomerInfoDto>()
				.ForMember(mem => mem.FirstName, opt => opt.MapFrom(src => src.Info.FirstName ?? string.Empty))
				.ForMember(mem => mem.LastName, opt => opt.MapFrom(src => src.Info.LastName ?? string.Empty))
				.ForMember(mem => mem.NationalCode, opt => opt.MapFrom(src => src.Info.NationalCode ?? string.Empty));
		}
	}
}
