using Application.Accounting.CustomerApp.Enums;
using Application.OrderManagement.Enums;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Infrastructure.DataManagements.MultiTenancyServices.TenantRegistry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.BackgroundServices
{
	internal sealed class CustomerEventsBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ITenantRegistryService _tenantRegistryService;
		private readonly ORMToolsOptions _options;

		public CustomerEventsBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<ORMToolsOptions> options, ITenantRegistryService tenantRegistryService)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
			_tenantRegistryService = tenantRegistryService ?? throw new ArgumentNullException(nameof(tenantRegistryService));
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = _serviceScopeFactory.CreateScope();

				var adminDb =
					scope.ServiceProvider
						 .GetRequiredService<AdminEfCoreDbContext>();

				var customerEvents = await adminDb.OutboxMessages
					.Where(@event => @event.Processed == false && @event.Type == OutBoxType.Customer && @event.RetryCount < 5)
					.ToListAsync(stoppingToken);

				if (!customerEvents.Any())
				{
					await Task.Delay(
						TimeSpan.FromSeconds(120),
						stoppingToken);

					continue;
				}

				var registryNeedsRefresh = false;


				foreach (var customerEvent in customerEvents)
				{
					try
					{
						switch (customerEvent.BehaviorType)
						{
							case OutboxBehaviorType.Create:
								{
									await HandleCreateCustomerEvent(customerEvent,stoppingToken);
									registryNeedsRefresh = true;
									break;
								}
							case OutboxBehaviorType.Update:
								break;
							case OutboxBehaviorType.Delete:
								break;
							case OutboxBehaviorType.DeleteAll:
								break;
							default:
								break;
						}
					}
					catch (Exception ex)
					{
						customerEvent.Error = ex.ToString();
						customerEvent.RetryCount++;
						if(customerEvent.RetryCount >= 5)
						{
							customerEvent.Processed = true;
							customerEvent.Error = " 5 RetryCounts hit";
							customerEvent.ProcessedAt = DateTimeOffset.UtcNow;
						}
					}

				}

				await adminDb.SaveChangesAsync(stoppingToken);
				if (registryNeedsRefresh)
				{
					await _tenantRegistryService.RefreshAsync();
				}
				await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);
			}

		}


		private async Task HandleCreateCustomerEvent(OutboxMessageModel customerEvent , CancellationToken cancellationToken)
		{

			var connectionString = string.Format(_options.EfCore.TenantConnectionString, customerEvent.TenantName);

			var options = new DbContextOptionsBuilder<TenantEfCoreDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			await using var tenantDb =
				new TenantEfCoreDbContext(options);

			await tenantDb.Database.MigrateAsync(cancellationToken);
			customerEvent.ProcessedAt = DateTimeOffset.UtcNow;
			customerEvent.Processed = true;
		}
	}
}
