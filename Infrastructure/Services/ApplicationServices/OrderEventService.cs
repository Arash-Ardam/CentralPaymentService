using Application.OrderManagement.Dtos.OrderEvent;
using Application.OrderManagement.Services;
using AutoMapper;
using Domain.Order;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices
{
	internal sealed class OrderEventService : IOrderEventService
	{
		private readonly TenantEfCoreDbContext _tenantDb;
		private readonly IMapper _mapper;
		public OrderEventService(TenantEfCoreDbContext tenantDb, IMapper mapper)
		{
			_tenantDb = tenantDb ?? throw new ArgumentNullException(nameof(tenantDb));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public Task AddAsync(OrderEventDto eventDto)
		{
			var model = _mapper.Map<OrderEventModel>(eventDto);
			_tenantDb.OrderEvents.AddAsync(model);
			return Task.CompletedTask;
		}

		public async Task<OrderEventDto> FindAsync(string orderId)
		{
			var model = await _tenantDb.OrderEvents.FirstOrDefaultAsync(e => e.OrderId == orderId);
			return _mapper.Map<OrderEventDto>(model);
		}

		public async Task<OrderEventDto> FindAsync(Guid eventId)
		{
			var model = await _tenantDb.OrderEvents.FirstOrDefaultAsync(e => e.Id == eventId);
			return _mapper.Map<OrderEventDto>(model);
		}
	}
}
