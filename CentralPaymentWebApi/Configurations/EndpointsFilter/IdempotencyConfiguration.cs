using CentralPaymentWebApi.EndpointFilters;
using Microsoft.OpenApi;

namespace CentralPaymentWebApi.Configurations.EndpointsFilter;

public static class IdempotencyConfiguration
{
	public static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder builder)
	{
		builder.AddEndpointFilter<IdempotencyEndpointFilter>();

		builder.WithMetadata(new RequireIdempotencyMetadata());

		return builder;
	}

	public static RouteHandlerBuilder RequireIdempotency<T>(this RouteHandlerBuilder builder)
	{
		builder.AddEndpointFilter<IdempotencyEndpointFilter<T>>();

		builder.WithMetadata(new RequireIdempotencyMetadata());

		return builder;
	}
}


public class RequireIdempotencyMetadata { }
