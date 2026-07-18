namespace Infrastructure.Services.Idempotency;


public interface IIdempotencyService
{
	Task<bool> AnyWithDifferentBodyAsync(string key, string body);

	Task<IdempotencyDto> GetIdempotencyRequestAsync(string key, string requstBody);
	Task<IdempotencyDto> AddIdempotentRequest(CreateIdempotencyRequestDto dto);
	Task RemoveIdempotencyRequest(string key);

	Task<IdempotencyDto> UpdateIdempotentRequest(UpdateIdempotencyRequestDto idempotencyDto);
}
