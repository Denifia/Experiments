using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibrarySharing;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and configures Library.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="configSectionPath">Path to the configuration section</param>
    /// <returns>The service collection for chaining</returns>
    /// <remarks>The default configuration section is "ServiceA"</remarks>
    public static IServiceCollection AddLibrary(this IServiceCollection serviceCollection, string configSectionPath = null)
    {
        return serviceCollection.AddLibrary(services =>
            services.AddOptions<ServiceAOptions>()
                .BindConfiguration(configSectionPath ?? ServiceAOptions.ConfigurationSectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart());
    }

    public static IServiceCollection AddLibrary(this IServiceCollection serviceCollection, Action<ServiceAOptions> configureOptions)
    {
        return serviceCollection.AddLibrary(services =>
            services.AddOptions<ServiceAOptions>()
                .Configure(configureOptions)
                .ValidateDataAnnotations()
                .ValidateOnStart());
    }

    public static IServiceCollection AddLibrary(this IServiceCollection services, ServiceAOptions userOptions)
    {
        return services.AddLibrary(options =>
        {
            options.Mode = userOptions.Mode;
            options.ReplyTo = userOptions.ReplyTo;
        });
    }

    private static IServiceCollection AddLibrary(this IServiceCollection services, Action<IServiceCollection> configureOptions)
    {
        services.AddScoped<IServiceA, ServiceA>();
        configureOptions(services);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ServiceAOptions>, ValidateServiceAOptions>());
        return services;
    }
}

public interface IServiceA
{
    void LogOptions();
}

internal class ServiceA : IServiceA
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

public sealed class ServiceAOptions
{
    public const string ConfigurationSectionName = "ServiceA";

    [Required, EnumDataType(typeof(Mode))]
    public Mode Mode { get; set; }

    [Required, EmailAddress]
    public string ReplyTo { get; set; }
}

public enum Mode
{
    None,
    Quick,
    Normal,
    ReallySlow
}

public class ValidateServiceAOptions : IValidateOptions<ServiceAOptions>
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
