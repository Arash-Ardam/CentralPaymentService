using Domain.Order;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataManagements.Repositories
{
	internal class OrderRepository : IOrderRepository
	{
		private readonly TenantEfCoreDbContext _dbContext;
		private readonly ORMToolsOptions _ormOptions;

		public OrderRepository(TenantEfCoreDbContext dbContext,IOptions<ORMToolsOptions> ormOptions)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_ormOptions = ormOptions.Value ?? throw new ArgumentNullException(nameof(ormOptions));

			_dbContext.Database.SetConnectionString(string.Format(_ormOptions.EfCore.TenantConnectionString, "Tenant")); // should be compeleted
		}

		public async Task<Order> CreateAsync(Order order)
		{
			await _dbContext.Orders.AddAsync(order);
			await _dbContext.SaveChangesAsync();

			return order;
		}

		public Task DeleteOrderAsync(string orderId)
		{
			throw new NotImplementedException();
		}

		public Task<List<Order>> GetAllOrdersAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<Order?> GetAsync(Guid id)
		{
			return await _dbContext.Orders
				.Include(ord =>  ord.SingleTransaction)
				.Include(ord => ord.GroupedTransactions)
				.FirstOrDefaultAsync(ord => ord.Id == id);
		}

		public async Task<Order?> GetByOrderIdsync(string orderId)
		{
			return await _dbContext.Orders
				.Include(ord => ord.SingleTransaction)
				.Include(ord => ord.GroupedTransactions)
				.FirstOrDefaultAsync(ord => ord.OrderId == orderId);
		}

		public async Task UpdateAsync(Order order)
		{
			_dbContext.Update(order);
			await _dbContext.SaveChangesAsync();
		}
	}
}
