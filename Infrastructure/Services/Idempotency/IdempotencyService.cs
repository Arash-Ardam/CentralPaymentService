using AutoMapper;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Idempotency
{
	internal sealed class IdempotencyService : IIdempotencyService
	{
		public IdempotencyService(TenantEfCoreDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		private readonly TenantEfCoreDbContext _dbContext;
		private readonly IMapper _mapper;

		public async Task<IdempotencyDto> AddIdempotentRequest(CreateIdempotencyRequestDto dto)
		{
			var createDto = IdempotencyRequestFactory.CreateIdempotencyRequest(dto);
			var model = _mapper.Map<IdempotencyModel>(createDto);
			var result = await _dbContext.AddAsync(model);
			await _dbContext.SaveChangesAsync();

			return _mapper.Map<IdempotencyDto>(result);
		}

		public Task<bool> AnyWithDifferentBodyAsync(string key, string body) =>
			_dbContext.IdempotencyRequests
			.AnyAsync(x => x.Key == key && x.RequestBody != body);

		public async Task<IdempotencyDto> GetIdempotencyRequestAsync(string key, string requstBody)
		{
			var model = await _dbContext.IdempotencyRequests
			.FirstOrDefaultAsync(
				x => x.Key == key &&
				x.RequestBody == requstBody);

			return _mapper.Map<IdempotencyDto>(model);
		}

		public async Task RemoveIdempotencyRequest(string key)
		{
			var model = await _dbContext.IdempotencyRequests.FirstOrDefaultAsync(x => x.Key == key);
			if (model == default) throw new ArgumentNullException("invalidKey");

			_dbContext.IdempotencyRequests.Remove(model);
		}

		public async Task<IdempotencyDto> UpdateIdempotentRequest(UpdateIdempotencyRequestDto idempotencyDto)
		{
			var model = await _dbContext.IdempotencyRequests.FirstOrDefaultAsync(x => x.Id == idempotencyDto.Id);

			if (model is null)
				throw new KeyNotFoundException("idempotent Id is invalid");

			var dto = IdempotencyRequestFactory.MarkRequest(idempotencyDto);
			model = _mapper.Map(dto,model);
			var result = _dbContext.IdempotencyRequests.Update(model);

			await _dbContext.SaveChangesAsync();

			return _mapper.Map<IdempotencyDto>(result);
		}
	}
}
