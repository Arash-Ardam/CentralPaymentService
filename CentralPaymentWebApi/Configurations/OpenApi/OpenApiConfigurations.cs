using CentralPaymentWebApi.Configurations.Identity;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Reflection;

namespace CentralPaymentWebApi.Configurations.OpenApi
{
	public static class OpenApiConfigurations
	{

		public static void AddOpenApiConfigs(this WebApplicationBuilder builder)
		{
			var idpSection = builder.Configuration.GetRequiredSection("IdentityOptions");
			var idpOption = idpSection.Get<IdentityOptions>();

			builder.Services.AddOpenApi(options =>
			{
				options.AddDocumentTransformer((document, context, cancellationToken) =>
				{
					document.Components ??= new OpenApiComponents();

					document.Components.SecuritySchemes ??=
						new Dictionary<string, IOpenApiSecurityScheme>();

					document.Components.SecuritySchemes["oauth2"] =
						new OpenApiSecurityScheme
						{
							Type = SecuritySchemeType.OpenIdConnect,

							OpenIdConnectUrl = new Uri(
								$"{idpOption.Authority}/.well-known/openid-configuration"
							),

							Description = "KeyCloak Login",
							Flows = new OpenApiOAuthFlows
							{
								Password = new OpenApiOAuthFlow
								{
									TokenUrl = new Uri($"{idpOption.Authority}/protocol/openid-connect/token"),
								}
							}
						};

					return Task.CompletedTask;
				});

			});
			builder.Services.AddSwaggerGen(opt =>
			{
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

				opt.IncludeXmlComments(xmlPath);
			});
		}

		public static void MapScalar(this WebApplication app)
		{
			var idpConfig = app.Configuration.GetRequiredSection("IdentityOptions").Get<IdentityOptions>();

			app.MapScalarApiReference(options =>
			{
				options.WithTitle("CentralPaymentWebApi");
				options.AddPreferredSecuritySchemes("oauth2");
				
			});
			
		}
	}
}
