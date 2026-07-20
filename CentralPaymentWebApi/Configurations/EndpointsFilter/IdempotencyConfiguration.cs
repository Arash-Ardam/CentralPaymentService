using CentralPaymentWebApi.EndpointFilters;

namespace CentralPaymentWebApi.Configurations.EndpointsFilter
{
	public static class IdempotencyConfiguration
	{
		public static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder builder)
		{
			builder.AddEndpointFilter<IdempotencyEndpointFilter>();
			return builder;
		}

		public static RouteHandlerBuilder RequireIdempotency<T>(this RouteHandlerBuilder builder)
		{
			builder.AddEndpointFilter<IdempotencyEndpointFilter<T>>();
			return builder;
		}
	}
}
