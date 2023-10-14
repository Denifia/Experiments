using CommandLine;
using CommandLiner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(RunOptions)
    .ContinueWith(x => x.Result.WithNotParsedAsync(HandleParseErrorAsync));

static async Task RunOptions(Options opts)
{
    var host = Host.CreateDefaultBuilder()
         .ConfigureServices((b, c) =>
         {
             c.AddTransient<Service>();
         })
         .ConfigureLogging(configure =>
         {
             configure.ClearProviders();
             configure.AddConsole().SetMinimumLevel(LogLevel.Warning);
         })
         .Build();

    var service = ActivatorUtilities.CreateInstance<Service>(host.Services);
    await service.DoTheThingAsync();
}

static Task HandleParseErrorAsync(IEnumerable<Error> errs)
{
    //handle errors
    return Task.CompletedTask;
}

class Options
{
    [Option(
      Default = false,
      HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; }
}
