using Application.Accounting.CustomerApp.Dtos;
using Infrastructure.DataManagements.DataModels;

namespace Infrastructure.Mappers
{
	internal sealed class CustomerEventMapper : MapperBase
	{
		public CustomerEventMapper()
		{
			CreateMap<CustomerEventDto, CustomerEventModel>()
				.ForMember(mem => mem.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
				.ForMember(mem => mem.RetryCount, opt => opt.MapFrom(src => 0))
				.ForMember(mem => mem.IsProccessed, opt => opt.MapFrom(src => false));
		}
	}
}
