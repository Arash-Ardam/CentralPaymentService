using CentralPaymentWebApi.Configurations.Identity;
using Scalar.AspNetCore;

namespace CentralPaymentWebApi.Configurations.OpenApi
{
	public static class OpenApiConfigurations
	{
		public static void MapScalar(this WebApplication app)
		{
			var idpConfig = app.Configuration.GetRequiredSection("IdentityOptions").Get<IdentityOptions>();

			app.MapScalarApiReference(options => options
				.AddPreferredSecuritySchemes("OAuth2")
				.AddAuthorizationCodeFlow("OAuth2", flow =>
				{
					flow.ClientId = idpConfig.ClientId;
					flow.ClientSecret = idpConfig.ClientSecret;
					flow.SelectedScopes = ["admin", "user"];
				})
			);
		}
	}
}
