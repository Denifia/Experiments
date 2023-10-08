using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<AnotherClass>();
builder.Services.AddHostedService<MainClass>();
var host = builder.Build();
host.Run();

class MainClass : IHostedService
{
    private delegate void NamedDelegate(string s);

    private NamedDelegate namedDelegate;
    private Action<string> action;
    private Func<string, string> func;
    private Predicate<string> predicate;

    private readonly ILogger<MainClass> logger;
    private readonly AnotherClass anotherClass;

    public MainClass(AnotherClass anotherClass, ILoggerFactory loggerFactory)
    {
        this.anotherClass = anotherClass;
        logger = loggerFactory.CreateLogger<MainClass>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConfigureToUseInjectedClass();
        CallTheDelegates();

        ConfigureToUseLambdas();
        CallTheDelegates();

        ConfigureToUseInlineDelegates();
        CallTheDelegates();

        ConfigureToChain();
        CallTheDelegates();

        return Task.CompletedTask;
    }

    private void CallTheDelegates()
    {
        var theString = "wow";
        logger.LogInformation("calling namedDelegate");
        namedDelegate(theString);

        logger.LogInformation("calling action");
        action(theString);

        logger.LogInformation("calling func");
        var funcReturn = func(theString);
        logger.LogInformation("the func returned {return}", funcReturn);

        logger.LogInformation("calling predicate");
        var predicateResult = predicate(theString);
        logger.LogInformation("the string equals \"wow\"? {result}", predicateResult);
    }

    private void ConfigureToUseInjectedClass()
    {
        namedDelegate = anotherClass.PrintsString;
        action = anotherClass.PrintsString;
        func = anotherClass.PrintsStringAndReturnsString;
        predicate = anotherClass.ChecksThatStringIsWow;

        logger.LogWarning("Using injected class!");
    }

    private void ConfigureToUseLambdas()
    {
        namedDelegate = (s) => logger.LogInformation("the string was \"{s}\"", s);

        action = (s) => logger.LogInformation("the string was \"{s}\"", s);

        func = (s) =>
        {
            logger.LogInformation("the string was \"{s}\"", s);
            return s;
        };

        predicate = (s) =>
        {
            logger.LogInformation("checking if string equals wow", s);
            return s == "wow";
        };

        logger.LogWarning("Using lambdas!");
    }

    private void ConfigureToUseInlineDelegates()
    {
        namedDelegate = delegate (string s)
        {
            logger.LogInformation("the string was \"{s}\"", s);
        };

        action = delegate (string s)
        {
            logger.LogInformation("the string was \"{s}\"", s);
        };

        func = delegate (string s)
        {
            logger.LogInformation("the string was \"{s}\"", s);
            return s;
        };

        predicate = delegate (string s)
        {
            logger.LogInformation("checking if string equals wow", s);
            return s == "wow";
        };

        logger.LogWarning("Using inline delegates!");
    }

    private void ConfigureToChain()
    {
        ConfigureToUseInjectedClass();

        // All the delegates will call anotherClass.<delegate>
        // Now we'll attach another invocation to the delegate

        namedDelegate += (s) => logger.LogInformation("!! the string was \"{s}\"", s);

        action += (s) => logger.LogInformation("!! the string was \"{s}\"", s);

        // Last attached returns to the caller
        func += (s) =>
        {
            logger.LogInformation("!! the string was \"{s}\"", s);
            return s;
        };

        // Last attached returns to the caller
        predicate += (s) =>
        {
            logger.LogInformation("!! checking if string equals wow", s);
            // returns false this time
            return s != "wow";
        };

        logger.LogWarning("Added lambdas to all delegates!");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

class AnotherClass
{
    private readonly ILogger logger;

    public AnotherClass(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<AnotherClass>();
    }

    public void PrintsString(string s)
    {
        logger.LogInformation("the string was \"{s}\"", s);
    }

    public string PrintsStringAndReturnsString(string s)
    {
        logger.LogInformation("the string was \"{s}\"", s);
        return s;
    }

    public bool ChecksThatStringIsWow(string s)
    {
        logger.LogInformation("checking if string equals wow", s);
        return s == "wow";
    }
}
