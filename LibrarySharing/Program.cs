using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LibrarySharing;

var builder = Host.CreateApplicationBuilder(args);

// Call the library extension method
builder.Services.AddLibrary();

builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();

class Worker : IHostedService
{
    private readonly IServiceA service;

    // Inject the library service
    public Worker(IServiceA service)
    {
        this.service = service;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        service.LogOptions();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
