using Application.Administration.Dtos.SingleOrder;
using Application.Administration.Services;
using Application.OrderManagement.Dtos.SingleOrder;
using AutoMapper;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;
using Infrastructure.DataManagements.MultiTenancyServices.TenantDbContextFactory;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ApplicationServices
{
	internal sealed class AdminOrderReportService : IAdminOrderReportService
	{
		private readonly ITenantDbContextFactory _tenantDbFactory;
		private readonly IMapper _mapper;
		public AdminOrderReportService(TenantEfCoreDbContext tenantEfCoreDbContext, IMapper mapper, ITenantDbContextFactory tenantDbFactory)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_tenantDbFactory = tenantDbFactory ?? throw new ArgumentNullException(nameof(tenantDbFactory));
		}
		public Task<List<SingleOrderReportDto>> FilterAsync(SingleOrderFilterDto filterDto)
		{
			var tenantDb = _tenantDbFactory.Create();

			var predicate = PredicateBuilder.New<SingleOrderReportModel>(true);

			if(! string.IsNullOrWhiteSpace(filterDto.OrderId))
				predicate = predicate.And(x => x.OrderId == filterDto.OrderId);

			if(! string.IsNullOrWhiteSpace(filterDto.OwnerAccount))
				predicate = predicate.And(x => x.SourceAccount == filterDto.OwnerAccount);

			if(! string.IsNullOrWhiteSpace(filterDto.DepositAccount))
				predicate = predicate.And(x => x.DepositAccount == filterDto.DepositAccount);

			if(filterDto.Amount.HasValue)
				predicate = predicate.And(x => x.Amount == filterDto.Amount);

			return tenantDb.SingleOrderReports
				.AsNoTracking()
				.Where(predicate)
				.Select(model =>_mapper.Map<SingleOrderReportDto>(model))
				.ToListAsync();
		}
	}
}
