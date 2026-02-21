using CentralPaymentWebApi.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CentralPaymentWebApi.Configurations.Identity;

public static class IdentityProviderConfiguration
{
	public static void AddApiAuthentication(this WebApplicationBuilder builder)
	{
		var idpSection = builder.Configuration.GetRequiredSection("IdentityOptions");
		var idpOption = idpSection.Get<IdentityOptions>();

		builder.Services.AddAuthentication(opt =>
		{
			opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.Authority = idpOption.Authority;
			options.Audience = idpOption.Audience;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidIssuer = idpOption.ValidIssuer,
				ValidTypes = new[] { "at+jwt" },
				NameClaimType = ClaimTypes.Name,
				RoleClaimType = ClaimTypes.Role
			};
		});
	}

	public static void AddApiAuthorization(this WebApplicationBuilder builder)
	{
		builder.Services.AddAuthorization(options =>
		{
			options.AddPolicy(AuthorizationConsts.AdminPolicy, policy =>
			{
				policy.RequireAssertion(rule => rule.User.HasClaim(claim =>
					(claim.Type == ClaimTypes.Role && claim.Value == AuthorizationConsts.AdminRole) ||
					(claim.Type == AuthorizationConsts.MicrosoftRoleType && claim.Value == AuthorizationConsts.AdminRole)));

			});

			options.AddPolicy(AuthorizationConsts.UserPolicy, policy =>
			{
				policy.RequireAssertion(rule => rule.User.HasClaim(claim =>
					(claim.Type == ClaimTypes.Role && claim.Value == AuthorizationConsts.UserRole) ||
					(claim.Type == AuthorizationConsts.MicrosoftRoleType && claim.Value == AuthorizationConsts.UserRole)
				));
			});
		});
	}
}
