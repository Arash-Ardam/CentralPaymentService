using Domain.Customer;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManagements.Repositories
{
	internal class CustomerRepository : ICustomerRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public CustomerRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;	
		}
		public async Task<Guid> AddAsync(Customer customer)
		{
			await _dbContext.Customers.AddAsync(customer);
			await _dbContext.SaveChangesAsync();

			return customer.Id;
		}

		public Task DeleteAsync(Guid Id)
		{
			throw new NotImplementedException();
		}

		public async Task EditAsync(Customer customer)
		{
			_dbContext.Customers.Update(customer);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<Customer> GetAsync(Guid id)
		{
			return await _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == id);
		}
	}
}
