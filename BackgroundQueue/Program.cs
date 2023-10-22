using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<UserInputMonitor>();
builder.Services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
builder.Services.AddHostedService<PeriodicQueueWorker>();
var host = builder.Build();

await host.StartAsync();

var userInputMonitor = host.Services.GetRequiredService<UserInputMonitor>();
userInputMonitor.Start();

await host.WaitForShutdownAsync();

internal class UserInputMonitor
{
    private readonly IBackgroundQueue _backgroundQueue;
    private readonly ILogger<UserInputMonitor> _logger;
    private readonly CancellationToken _cancellationToken;

    public UserInputMonitor(
        IBackgroundQueue backgroundQueue,
        ILogger<UserInputMonitor> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _backgroundQueue = backgroundQueue;
        _logger = logger;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    public void Start()
    {
        _logger.LogInformation($"{nameof(UserInputMonitor)} starting. Press \'w\' key to enqueue an item.");

        // Run a console user input loop in a background thread
        Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            var keyStroke = Console.ReadKey();

            if (keyStroke.Key == ConsoleKey.W)
            {
                // Enqueue a background work item
                await _backgroundQueue.EnqueueAsync(Guid.NewGuid().ToString());
            }
        }
    }
}

internal interface IBackgroundQueue
{
    ValueTask EnqueueAsync(string item);

    bool TryReadAsync(out string item);
}

internal class BackgroundQueue : IBackgroundQueue
{
    private readonly Channel<string> _queue;

    public BackgroundQueue()
    {
        _queue = Channel.CreateUnbounded<string>();
    }

    public ValueTask EnqueueAsync(string item)
        => _queue.Writer.WriteAsync(item);

    public bool TryReadAsync(out string item)
    {
        var success = _queue.Reader.TryRead(out string? i);
        item = i!;
        return success;
    }
}

internal class PeriodicQueueWorker : BackgroundService
{
    private readonly IBackgroundQueue _backgroundQueue;
    private readonly ILogger<PeriodicQueueWorker> _logger;

    public PeriodicQueueWorker(
        IBackgroundQueue backgroundQueue,
        ILogger<PeriodicQueueWorker> logger)
    {
        _backgroundQueue = backgroundQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(PeriodicQueueWorker)} starting.");
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            while (_backgroundQueue.TryReadAsync(out string item))
            {
                _logger.LogInformation(item);
            }
        }

        _logger.LogInformation($"{nameof(PeriodicQueueWorker)} shutting down.");
    }
}
