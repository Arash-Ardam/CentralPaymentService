using Application.OrderManagement.Dtos.OrderEvent;
using AutoMapper;
using Infrastructure.DataManagements.DataModels;

namespace Infrastructure.Mappers
{
	internal sealed class OrderEventMapper : Profile
	{
		public OrderEventMapper()
		{
			CreateMap<OrderEventDto, OrderEventModel>()
				.ForMember(mem => mem.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
				.ForMember(mem => mem.Processed, opt => opt.MapFrom(src => false));

			CreateMap<OrderEventModel, OrderEventDto>();
		}
	}
}
