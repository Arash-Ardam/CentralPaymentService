namespace Application.OrderManagement.Services
{
	public interface IUnitOfWork
	{
		Task SaveAdminChangesAsync(
			CancellationToken cancellationToken = default);

		Task SaveTenantChangesAsync(
		CancellationToken cancellationToken = default);
	}
}
