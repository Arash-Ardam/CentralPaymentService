using Domain.Order;
using Infrastructure.DataManagements.Abstractions;
using Infrastructure.DataManagements.MultiTenancyServices.TenantResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataManagements.Repositories
{
	internal class OrderRepository : IOrderRepository
	{
		private readonly TenantEfCoreDbContext _dbContext;
		private readonly ITenantResolver _tenantResolver;

		public OrderRepository(TenantEfCoreDbContext dbContext,ITenantResolver tenantResolver)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));

			SetConnectionString();
		}

		private void SetConnectionString()
		{
			var coonectionString = _tenantResolver.Resolve();
			Console.WriteLine($"Tenant dbContext configed : {coonectionString}");
			_dbContext.Database.SetConnectionString(coonectionString);
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
