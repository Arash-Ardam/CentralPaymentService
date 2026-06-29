using Application.Abstractions.Dtos;
using Infrastructure.DataManagements.DataModels;

namespace Infrastructure.Mappers
{
	internal class OutboxMessageMapper : MapperBase
	{
		public OutboxMessageMapper()
		{
			CreateMap<OutboxMessageDto, OutboxMessageModel>()
				.ForMember(mem => mem.Processed, opt => opt.MapFrom(src => false))
				.ForMember(mem => mem.RetryCount, opt => opt.MapFrom(src => 0))
				.ForMember(mem => mem.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

			CreateMap<OutboxMessageModel, OutboxMessageDto>();
		}
	}
}
