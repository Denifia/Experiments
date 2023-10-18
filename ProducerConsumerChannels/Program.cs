using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(100);
            await _channel.Produce(i.ToString());
        }

        //while (true)
        //{
        //    await Task.Delay(100);
        //    await _channel.Produce(Random.Shared.Next().ToString());
        //}
    }
}

class Consumer1 : BackgroundService
{
    private readonly MessagesChannel _channel;
    private readonly string _name;

    public Consumer1(MessagesChannel channel, string name = "1")
    {
        _channel = channel;
        _name = $" consumed by {name}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Consume())
        {
            Console.WriteLine(message + _name);
        }
    }
}

class Consumer2 : Consumer1
{
    public Consumer2(MessagesChannel channel) : base(channel, "2") { }
}

class MessagesChannel
{
    private Channel<string> _channel = Channel.CreateUnbounded<string>();
    public ValueTask Produce(string message) => _channel.Writer.WriteAsync(message);
    public IAsyncEnumerable<string> Consume() => _channel.Reader.ReadAllAsync();
}
