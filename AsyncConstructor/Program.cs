try
{
    var awesome = new AwesomeClass();
    awesome.DoThingA();
    await awesome.DoThingB();
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.Out.WriteLine(ex.Message);
    Console.Out.WriteLine("shutting down.");
}

public class AwesomeClass
{
    private readonly Task _prepTask;
    private async Task<bool> IsPrepared()
    {
        await _prepTask;
        return _prepTask.IsCompletedSuccessfully;
    }

    public AwesomeClass()
    {
        Console.Out.WriteLine("AwesomeClass constructing...");

        // calling an async method in a constructor
        _prepTask = PrepAsync();

        Console.Out.WriteLine("AwesomeClass constructed!");
    }

    private async Task PrepAsync()
    {
        Console.Out.WriteLine("AwesomeClass preparing...");
        await InnerPrepAsync();
        Console.Out.WriteLine("AwesomeClass prepared.");
    }

    private async Task InnerPrepAsync()
    {
        await Task.Delay(2000);

        // See what happens when you uncomment this
        //throw new Exception("something bad happened");
    }

    internal void DoThingA()
    {
        // methods may not care about the constructor prep
        Console.Out.WriteLine("did thing A");
    }

    internal async Task DoThingB()
    {
        // optionally checking if the constructor prep worked
        await IsPrepared();
        Console.Out.WriteLine("did thing B");
    }
}
