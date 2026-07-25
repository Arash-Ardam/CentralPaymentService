namespace CentralPaymentWebApi.Handlers
{
	public interface IIdempotencyHandler
	{
		Task<bool> BeginRequestAsync(EndpointFilterInvocationContext context);

		IResult? GetCachedResult();

		Task<IResult> EndRequestAsync(object? endpointResult);
		Task<IResult> EndRequestAsync<T>(object? endpointResult);
	}
}
