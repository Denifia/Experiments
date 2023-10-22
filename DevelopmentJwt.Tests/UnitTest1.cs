using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.TestHost;

namespace DevelopmentJwt.Tests;

public class UnitTest1 : IClassFixture<ApiApplicationFactory>
{
    private readonly ApiApplicationFactory _factory;

    public UnitTest1(ApiApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UserCanAccessRandomNumber()
    {
        // Arrange
        var user = CreateTestUser(roles: "user");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => services.AddScoped(_ => user));
        }).CreateClient();

        // Act
        var response = await client.GetAsync("random-number");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserCanAccessBiggerRandomNumber()
    {
        // Arrange
        var user = CreateTestUser(roles: "user");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => services.AddScoped(_ => user));
        }).CreateClient();

        // Act
        var response = await client.GetAsync("bigger-random-number");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserCannotAccessSecretNumber()
    {
        // Arrange
        var user = CreateTestUser(roles: "user");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => services.AddScoped(_ => user));
        }).CreateClient();

        // Act
        var response = await client.GetAsync("secret-number");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private MockAuthUser CreateTestUser(params string[] roles)
    {
        var mockUser = new MockAuthUser();
        foreach (var role in roles)
        {
            mockUser.Claims.Add(new Claim("role", role));
        }
        return mockUser;
    }
}
