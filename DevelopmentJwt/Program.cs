using Microsoft.AspNetCore.Authentication.JwtBearer;

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
