using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Threading.Channels;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<MessagesChannel>();
builder.Services.AddHostedService<Producer>();
builder.Services.AddHostedService<Consumer1>();
builder.Services.AddHostedService<Consumer2>();
var host = builder.Build();
host.Run();

class Producer : BackgroundService
{
    private readonly MessagesChannel _channel;

    public Producer(MessagesChannel channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (int i = 0; i < 200; i++)
        {
            await Task.Delay(100);
            await _channel.Publish(i.ToString());
        }
    }
}

class Consumer1 : BackgroundService
{
    private readonly ChannelReader<string> _channel;
    private readonly string _name;
    private readonly int _msDelay;

    public Consumer1(MessagesChannel messagesChannel, int msDelay = 0, string name = "1")
    {
        _channel = messagesChannel.Subscribe(name);
        _name = $" consumed by {name}";
        _msDelay = msDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.ReadAllAsync())
        {
            await Task.Delay(_msDelay);
            Console.WriteLine(message + _name);
        }
    }
}

class Consumer2 : Consumer1
{
    public Consumer2(MessagesChannel channel) : base(channel, msDelay: 500, "2") { }
}

class MessagesChannel
{
    private ConcurrentDictionary<string, Channel<string>> _subscriberChannels = new ConcurrentDictionary<string, Channel<string>>();

    public async ValueTask Publish(string message)
    {
        foreach (var channel in _subscriberChannels.Values)
        {
            await channel.Writer.WriteAsync(message);
        }
    }

    public ChannelReader<string> Subscribe(string name)
    {
        _subscriberChannels.TryAdd(name, Channel.CreateUnbounded<string>());
        return _subscriberChannels[name];
    }
}
