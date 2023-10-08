using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
var host = builder.Build();

var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Example log message");

using (var loggerScope = logger.BeginScope("[important scope]"))
{
    logger.LogInformation("Did a thing");
    logger.LogInformation("Did more things");

    using (var innerLoggerScope = logger.BeginScope("[very important]"))
    {
        logger.LogInformation("Even more things");
    }

    logger.PlaceOfResidence("billy", "perth");
}

public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "{name} lives in {city}.")]
    public static partial void PlaceOfResidence(
        this ILogger logger,
        string name,
        string city);
}
