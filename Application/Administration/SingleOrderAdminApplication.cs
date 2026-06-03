using Application.Abstractions;
using Application.Accounting.CustomerApp.Services;
using Application.Administration.Dtos.SingleOrder;
using Application.Administration.Services;
using Application.OrderManagement.Dtos.SingleOrder;
using Application.OrderManagement.Services;

namespace Application.Administration
{
	
	internal sealed class SingleOrderAdminApplication : ISingleOrderAdminApplication
	{
		private readonly ITenantContext _tenantContext;
		private readonly IAdminOrderReportService _orderReportService;
		private readonly ICustomerQueryService _customerQueryService;
		public SingleOrderAdminApplication(ITenantContext tenantContext, IAdminOrderReportService orderReportService, ICustomerQueryService customerQueryService)
		{
			_tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
			_orderReportService = orderReportService ?? throw new ArgumentNullException(nameof(orderReportService));
			_customerQueryService = customerQueryService ?? throw new ArgumentNullException(nameof(customerQueryService));
		}

		public async Task<ApplicationResponse<List<SingleOrderReportDto>>> FilterAsync(SingleOrderFilterDto filterDto)
		{
			var response = new ApplicationResponse<List<SingleOrderReportDto>>() { IsSuccess = true };
			try
			{
				var targetTenant = await _customerQueryService.GetAsync(filterDto.TenantId);
				if(targetTenant is null)
				{
					response.IsSuccess = false;
					response.Message = "Tenant not found.";
					response.Status = ApplicationResultStatus.NotFound;
					return response;
				}

				_tenantContext.SetTenantByAdmin(targetTenant.TenantName);

				var report = await _orderReportService.FilterAsync(filterDto);
				response.Data = report;
				response.Status = ApplicationResultStatus.Done;
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = $"An error occurred while filtering orders: {ex.Message}";
				response.Status = ApplicationResultStatus.Exception;
				return response;
			}
		} 


	}
}
