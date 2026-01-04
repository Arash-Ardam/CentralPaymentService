

namespace Domain.Order;


public interface IOrderRepository
{
	Task<Order> CreateAsync(Order order);
	
	Task UpdateOrderAsync(Order order);

	Task DeleteOrderAsync(string orderId);

	Task<Order?> GetAsync(Guid id);

	Task<Order?> GetByOrderIdsync(string orderId);

	Task<List<Order>> GetAllOrdersAsync();
}
