using Infrastructure.DataManagements.Abstractions.ORMs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.BackgroundServices
{
	internal sealed class IdempotencyExpirationBackgroundService : BackgroundService
	{
		public IdempotencyExpirationBackgroundService(IServiceScopeFactory serviceProvider)
		{
			_serviceFactory = serviceProvider;
		}

		private readonly IServiceScopeFactory _serviceFactory;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = _serviceFactory.CreateScope();

				var adminDb =
					scope.ServiceProvider
						 .GetRequiredService<AdminEfCoreDbContext>();

				var expiredIdempotentRequests = adminDb.IdempotencyRequests
					.Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow);

				if (expiredIdempotentRequests.Any())
					adminDb.IdempotencyRequests.RemoveRange(expiredIdempotentRequests);

				await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
			}
		}
	}
}
