using Application.OrderManagement.Dtos.OrderEvent;

namespace Application.OrderManagement.Services
{
	public interface IOrderEventService
	{
		Task AddAsync(OrderEventDto eventDto);

		Task<OrderEventDto> FindAsync(string orderId);
		Task<OrderEventDto> FindAsync(Guid eventId);
	}
}
