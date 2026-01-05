using Domain.Customer;
using Infrastructure.DataManagements.Abstractions;

namespace Infrastructure.DataManagements.Repositories
{
	internal class CustomerRepository : ICustomerRepository
	{
		private readonly EfCoreDbContext _dbContext;

		public CustomerRepository(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext;	
		}
		public Task<Guid> AddAsync(Customer customer)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(Guid Id)
		{
			throw new NotImplementedException();
		}

		public Task EditAsync(Customer customer)
		{
			throw new NotImplementedException();
		}

		public Task<Customer> GetAsync(Guid id)
		{
			throw new NotImplementedException();
		}
	}
}
