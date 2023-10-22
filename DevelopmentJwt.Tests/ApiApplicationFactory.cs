using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace DevelopmentJwt.Tests;

public class ApiApplicationFactory : WebApplicationFactory<DevelopmentJwt.Program>
{

    // Default logged in user for all requests - can be overwritten in individual tests
    private readonly MockAuthUser _user = new MockAuthUser(
        new Claim("sub", Guid.NewGuid().ToString()),
        new Claim("email", "default-user@xyz.com"));

    public ApiApplicationFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureServices(services =>
        {
            services.AddTestAuthentication();
            services.AddScoped(_ => _user);

            // Build the service provider.
            //var sp = services.BuildServiceProvider();
        });
    }
}
