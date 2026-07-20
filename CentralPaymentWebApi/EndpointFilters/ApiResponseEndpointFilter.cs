using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.Handlers;

namespace CentralPaymentWebApi.EndpointFilters
{
	public class ApiResponseEndpointFilter : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var result = await next(context);

			if (result is not ApiResponse apiResponse)
				return Results.Conflict("Endpoint result type conflicts");

			return apiResponse.HttpResult;
		}
	}

	public class ApiResponseEndpointFilter<T> : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var result = await next(context);

			if (result is not ApiResponse<T> apiResponse)
				return Results.Conflict("Endpoint result type conflicts");

			return apiResponse.HttpResult;
		}
	}
}
