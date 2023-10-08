using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddScoped<ServiceA>()
    .AddOptions<ServiceAOptions>()
    .Bind(builder.Configuration.GetSection(ServiceAOptions.ConfigurationSectionName), o => o.ErrorOnUnknownConfiguration = true)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ServiceAOptions>, ValidateServiceAOptions>());

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

class Worker : IHostedService
{
    private readonly ServiceA service;

    public Worker(ServiceA service)
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

class ServiceA
{
    private readonly IOptionsSnapshot<ServiceAOptions> options;
    private readonly ILogger<ServiceA> logger;

    public ServiceA(IOptionsSnapshot<ServiceAOptions> options, ILogger<ServiceA> logger)
    {
        this.options = options;
        this.logger = logger;
    }

    public void LogOptions()
    {
        logger.LogInformation("Mode is {mode}", options.Value.Mode);
        logger.LogInformation("ReplyTo is {email}", options.Value.ReplyTo);
    }
}

class ServiceAOptions
{
    public const string ConfigurationSectionName = "ServiceA";

    [Required]
    [EnumDataType(typeof(Mode))]
    public Mode Mode { get; set; }

    [Required]
    [EmailAddress]
    public string ReplyTo { get; set; }
}

public enum Mode
{
    None,
    Quick,
    Normal,
    ReallySlow
}

class ValidateServiceAOptions : IValidateOptions<ServiceAOptions>
{
    public ValidateOptionsResult Validate(string name, ServiceAOptions options)
    {
        StringBuilder failure = null;

        if (options.Mode == Mode.None)
        {
            (failure ??= new()).AppendLine($"Mode is required and cannot be Mode.None");
        }

        return failure is not null
            ? ValidateOptionsResult.Fail(failure.ToString())
            : ValidateOptionsResult.Success;
    }
}
