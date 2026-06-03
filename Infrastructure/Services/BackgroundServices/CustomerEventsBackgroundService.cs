using Application.Accounting.CustomerApp.Enums;
using Infrastructure.DataManagements;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.BackgroundServices
{
	internal sealed class CustomerEventsBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ORMToolsOptions _options;

		public CustomerEventsBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<ORMToolsOptions> options)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			_options = options.Value ?? throw new ArgumentNullException(nameof(options));
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = _serviceScopeFactory.CreateScope();

				var options = new DbContextOptionsBuilder<AdminEfCoreDbContext>()
					.UseSqlServer(_options.EfCore.BaseConnectionString)
					.Options;

				await using var adminDb =
					new AdminEfCoreDbContext(options);

				var customerEvents = await adminDb.CustomerEvents
					.Where(@event => @event.IsProccessed == false)
					.ToListAsync(stoppingToken);

				if (!customerEvents.Any())
				{
					await Task.Delay(
						TimeSpan.FromSeconds(120),
						stoppingToken);

					continue;
				}

				foreach (var customerEvent in customerEvents)
				{
					if (customerEvent.RetryCount > 5)
					{
						customerEvent.IsProccessed = true;
						customerEvent.Error = " 5 RetryCounts hit";
						customerEvent.ProccessedAt = DateTimeOffset.UtcNow;
						continue;
					}

					try
					{
						switch (customerEvent.Type)
						{
							case CustomerEventType.Create:
								await HandleCreateCustomerEvent(customerEvent);
								break;
							case CustomerEventType.Update:
								break;
							case CustomerEventType.Delete:
								break;
							case CustomerEventType.DeleteAll:
								break;
							default:
								break;
						}
					}
					catch (Exception ex)
					{
						customerEvent.Error = ex.ToString();
						customerEvent.RetryCount++;
					}

				}

				await adminDb.SaveChangesAsync(stoppingToken);
				await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);
			}

		}


		private async Task HandleCreateCustomerEvent(CustomerEventModel customerEvent)
		{
			var connectionString = string.IsNullOrWhiteSpace(customerEvent.ConnectionString) ?
									string.Format(_options.EfCore.TenantConnectionString, customerEvent.TenantName) : customerEvent.ConnectionString;

			var options = new DbContextOptionsBuilder<TenantEfCoreDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			await using var tenantDb =
				new TenantEfCoreDbContext(options);

			if (!tenantDb.Database.CanConnect())
				await tenantDb.Database.MigrateAsync();
			else
				customerEvent.Error = "Tenant Database already migrated";

				customerEvent.ProccessedAt = DateTimeOffset.UtcNow;
				customerEvent.IsProccessed = true;
		}
	}
}
