using Application.Accounting.CustomerApp.Dtos;
using Application.Accounting.CustomerApp.Services;
using Infrastructure.DataManagements.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices.QueryServices
{
	internal class CustomerQueryService : ICustomerQueryService
	{
		private readonly AdminEfCoreDbContext _dbContext;
		public CustomerQueryService(AdminEfCoreDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public Task<List<CustomerInfoDto>> GetAllAsync()
		{
			return (
				from customer in _dbContext.Customers.AsNoTracking()
				select new CustomerInfoDto
				{
					FirstName = customer.Info.FirstName,
					LastName = customer.Info.LastName,
					Id = customer.Id,
					NationalCode = customer.Info.NationalCode,
					TenantName = customer.TenantName,
					IsEnable = customer.IsEnable
				}).ToListAsync();
		}

		public Task<CustomerInfoDto?> GetAsync(Guid id)
		{
			return (
				from customer in _dbContext.Customers.AsNoTracking()
				where customer.Id == id
				select new CustomerInfoDto
				{
					FirstName = customer.Info.FirstName,
					LastName = customer.Info.LastName,
					Id = customer.Id,
					NationalCode = customer.Info.NationalCode,
					TenantName = customer.TenantName,
					IsEnable = customer.IsEnable
				}).FirstOrDefaultAsync();
		}

		public Task<bool> IsExists(string tenantName)
		{
			return _dbContext.Customers.AsNoTracking().AnyAsync(x => x.TenantName == tenantName);
		}
	}
}
