using CentralPaymentWebApi.Abstractions;
using CentralPaymentWebApi.MinimalApis.Accounting;
using CentralPaymentWebApi.MinimalApis.Payment;

namespace CentralPaymentWebApi.MinimalApis
{
	public static class ApiRegistrations
	{
		public static void MapAccountingApis(this WebApplication app)
		{
			app.MapGroup("api/customers")
			   .WithTags("Customers")
			   .WithSummary("this is set of apis for managing customers")
			   .MapCustomerApis()
			   .RequireAuthorization(AuthorizationConsts.AdminPolicy)
			   .WithRequestTimeout(timeout: TimeSpan.FromSeconds(30));

			app.MapGroup("api/banks")
				.WithTags("Banks")
				.WithSummary("this is set of apis for managing banks")
				.MapBankApis()
				.RequireAuthorization(AuthorizationConsts.AdminPolicy)
				.WithRequestTimeout(timeout: TimeSpan.FromSeconds(30));

			app
				.MapGroup("api/accounts")
				.WithTags("Accounts")
				.WithSummary("this is set of apis for managing accounts")
				.MapAccountApis()
				.RequireAuthorization(AuthorizationConsts.AdminPolicy)
				.WithRequestTimeout(timeout: TimeSpan.FromSeconds(30));
		}

		public static void MapPaymentApis(this WebApplication app)
		{
			app
				.MapGroup("api/payments/single")
				.WithTags("SinglePaymentOrders")
				.WithSummary("this is set of apis for managing single payment orders")
				.MapSinglePaymentApis()
				.RequireAuthorization(AuthorizationConsts.UserPolicy)
				.WithRequestTimeout(timeout: TimeSpan.FromSeconds(60));
		}
	}
}
