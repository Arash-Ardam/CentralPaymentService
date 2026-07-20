using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Handlers;

namespace CentralPaymentWebApi.EndpointFilters
{
	public class IdempotencyEndpointFilter : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var handler =
		   context.HttpContext.RequestServices
			   .GetRequiredService<IIdempotencyHandler>();

			var canContinue = await handler.BeginRequestAsync(context.HttpContext);

			if (!canContinue)
				return handler.GetCachedResult();

			var result = await next(context);

			return await handler.EndRequestAsync(result);
		}
	}

	public class IdempotencyEndpointFilter<T> : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var handler =
		   context.HttpContext.RequestServices
			   .GetRequiredService<IIdempotencyHandler>();

			var canContinue = await handler.BeginRequestAsync(context.HttpContext);

			if (!canContinue)
				return handler.GetCachedResult();

			var result = await next(context);

			return await handler.EndRequestAsync<T>(result);
		}
	}
}
