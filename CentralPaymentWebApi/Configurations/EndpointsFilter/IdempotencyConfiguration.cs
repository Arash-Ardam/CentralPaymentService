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
	}
}
