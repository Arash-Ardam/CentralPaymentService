using Application.Abstractions.Dtos;

namespace Application.Abstractions.Services
{
	public interface IOutboxMessageService
	{
		Task PublishAsync(OutboxMessageDto eventDto);

		Task<OutboxMessageDto> FindAsync(string outboxId);
		Task<OutboxMessageDto> FindAsync(Guid id);
	}
}
