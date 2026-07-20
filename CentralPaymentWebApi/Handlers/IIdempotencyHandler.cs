namespace CentralPaymentWebApi.Handlers
{
	public interface IIdempotencyHandler
	{
		Task<bool> BeginRequestAsync(HttpContext context);

		IResult? GetCachedResult();

		Task<IResult> EndRequestAsync(object? endpointResult);
	}
}
