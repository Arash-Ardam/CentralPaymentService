using Infrastructure.DataManagements.DataModels;
using Infrastructure.Services.Idempotency;

namespace Infrastructure.Mappers;

internal sealed class IdempotencyMapper : MapperBase
{
	public IdempotencyMapper()
	{
		CreateMap<UpdateIdempotencyRequestDto, IdempotencyModel>()
			.ForMember(mem => mem.ExpiresAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow.AddDays(2)));

		CreateMap<IdempotencyModel, IdempotencyDto>();

		CreateMap<IdempotencyDto, IdempotencyModel>();
	}
}
