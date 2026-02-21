using Application.OrderManagement.Services;

namespace CentralPaymentWebApi.Middlewares
{
	public class TenantContextMiddleware
	{
		public RequestDelegate Next { get; }

		public TenantContextMiddleware(RequestDelegate next)
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var tenantContext = context.RequestServices.GetService<ITenantContext>();

			if (context.User.IsInRole("User"))
			{
				tenantContext.SetTenantByUser();
			}

			await Next(context);
		}

	}
}
