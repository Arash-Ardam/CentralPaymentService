namespace Infrastructure.Services.Idempotency
{
	internal static class IdempotencyRequestFactory
	{
		public static IdempotencyDto CreateIdempotencyRequest(CreateIdempotencyRequestDto createDto)
		{
			return new IdempotencyDto
			{
				Key = createDto.Key,
				RequestBody = createDto.RequestBody,
				Status = IdempotencyStatus.Pending,
				CreatedAt = DateTimeOffset.UtcNow,
				ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(2),
			};
		}


		public static IdempotencyDto MarkRequest(UpdateIdempotencyRequestDto updateDto)
		{
			return new IdempotencyDto
			{
				Id = updateDto.Id,
				Key = updateDto.Key,
				RequestBody = updateDto.RequestBody,
				ResponseBody = updateDto.ResponseBody,
				StatusCode = updateDto.StatusCode,
				Status = updateDto.Status,
				ExpiresAt = updateDto.Status switch
				{
					IdempotencyStatus.Pending => DateTimeOffset.UtcNow.AddMinutes(2),
					IdempotencyStatus.Completed => DateTimeOffset.UtcNow.AddDays(2),
					IdempotencyStatus.Rejected => DateTimeOffset.UtcNow.AddDays(1),
					IdempotencyStatus.Failed => DateTimeOffset.UtcNow.AddMinutes(2),
					_ => DateTimeOffset.UtcNow.AddMinutes(30)
				},
			};
		}
	}
}
