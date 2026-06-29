using Application.Abstractions.Dtos;
using Application.Abstractions.Services;
using AutoMapper;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices.EventServices
{
	internal class OutboxMessageService : IOutboxMessageService
	{
		public AdminEfCoreDbContext _dbContext { get; }
		public IMapper _mapper { get; }

		public OutboxMessageService(AdminEfCoreDbContext adminEfCoreDbContext, IMapper mapper)
		{
			_dbContext = adminEfCoreDbContext;
			_mapper = mapper;
		}

		public async Task PublishAsync(OutboxMessageDto outboxMessageDto)
		{
			var outboxModel = _mapper.Map<OutboxMessageModel>(outboxMessageDto);
			await _dbContext.OutboxMessages.AddAsync(outboxModel);
		}

		public async Task<OutboxMessageDto> FindAsync(string outboxId)
		{
			var outbox = await _dbContext.OutboxMessages.FirstOrDefaultAsync(msg => msg.OutboxId == outboxId);
			return _mapper.Map<OutboxMessageDto>(outbox);
		}

		public async Task<OutboxMessageDto> FindAsync(Guid id)
		{
			var outbox = await _dbContext.OutboxMessages.FirstOrDefaultAsync(msg => msg.Id == id);
			return _mapper.Map<OutboxMessageDto>(outbox);
		}
	}
}
