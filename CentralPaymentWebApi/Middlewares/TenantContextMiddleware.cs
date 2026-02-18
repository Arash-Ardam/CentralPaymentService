using Application.OrderManagement.Services;

namespace CentralPaymentWebApi.Middlewares
{
	public class TenantContextMiddleware
	{
		public RequestDelegate Next { get; }
		public ITenantContext TenantContext { get; }

		public TenantContextMiddleware(RequestDelegate next,ITenantContext tenantContext)
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));
			TenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
		}

		public async Task InvokeAsync(HttpContext context,ITenantContext tenantContext)
		{
			if (context.User.IsInRole("User"))
			{
				tenantContext.SetTenantByUser();
			}

			await Next(context);
		}

	}
}
