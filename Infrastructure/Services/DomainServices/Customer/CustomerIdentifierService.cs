using Domain.Customer.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.DomainServices.Customer
{
	internal class CustomerIdentifierService : ICustomerIdentifierService
	{
		public CustomerIdentifierService(EfCoreDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public EfCoreDbContext _dbContext { get; }

		public Task<bool> isCustomerExists(string tenantName)
		{
			return _dbContext.Customers.AnyAsync(x => x.TenantName == tenantName);
		}
	}
}
