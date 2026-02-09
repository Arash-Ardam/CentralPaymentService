namespace Domain.Customer;

public interface ICustomerRepository
{
	Task<Guid> AddAsync(Customer customer);
	Task EditAsync(Customer customer);

	Task<Customer> GetAsync(Guid id);

	Task DeleteAsync(Guid Id);

	Task<List<Customer>> GetAllAsync();
}
