using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace DevelopmentJwt.Tests;

public static class AuthServiceCollectionExtensions
{
    public static AuthenticationBuilder AddTestAuthentication(this IServiceCollection services)
    {
        //services.AddAuthorization(options =>
        //{
        //    //options.DefaultPolicy = new AuthorizationPolicyBuilder(AuthConstants.Scheme)
        //    //    .RequireAuthenticatedUser()
        //    //    .Build();

        //    options.AddPolicy("user", policy => policy.RequireRole("user"));
        //});

        return services.AddAuthentication(AuthConstants.Scheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(AuthConstants.Scheme, options => { });
    }
}
