using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DevelopmentJwt;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("user", policy => policy.RequireRole("user", "admin"));
            options.AddPolicy("admin", policy => policy.RequireRole("admin"));
        });

        var app = builder.Build();

        app.UseAuthorization();

        app.MapGet("/random-number", () => Random.Shared.Next(0, 11).ToString());
        app.MapGet("/bigger-random-number", () => Random.Shared.Next(100, 1001).ToString()).RequireAuthorization("user");
        app.MapGet("/secret-number", () => "42").RequireAuthorization("admin");

        app.Run();
    }
}
