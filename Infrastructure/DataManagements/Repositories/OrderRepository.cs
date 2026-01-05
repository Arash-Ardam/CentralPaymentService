using Domain.Order;
using Infrastructure.DataManagements.Abstractions;

namespace Infrastructure.DataManagements.Repositories
{
	internal class OrderRepository : IOrderRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public OrderRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<Order> CreateAsync(Order order)
		{
			throw new NotImplementedException();
		}

		public Task DeleteOrderAsync(string orderId)
		{
			throw new NotImplementedException();
		}

		public Task<List<Order>> GetAllOrdersAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Order?> GetAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<Order?> GetByOrderIdsync(string orderId)
		{
			throw new NotImplementedException();
		}

		public Task UpdateOrderAsync(Order order)
		{
			throw new NotImplementedException();
		}
	}
}
