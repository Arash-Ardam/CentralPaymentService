using CentralPaymentWebApi.EndpointFilters;

namespace CentralPaymentWebApi.Configurations.EndpointsFilter
{
	public static class ApiResponseFilterConfiguration
	{
		public static RouteHandlerBuilder WithApiResponse(this RouteHandlerBuilder builder)
		{
			builder.AddEndpointFilter<ApiResponseEndpointFilter>();
			return builder;
		}


		public static RouteHandlerBuilder WithApiResponse<T>(this RouteHandlerBuilder builder)
		{
			builder.AddEndpointFilter<ApiResponseEndpointFilter<T>>();

			return builder;
		}
	}
}
