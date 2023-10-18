// you can add a timeout to the source that will trigger auto cancel
CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(1));

// `.Token` makes a new struct
var token = cancellationTokenSource.Token;

// token is copied to methods 'cause it's a struct
var task1 = DoWork(token); 
var task2 = DoWorkWithCancelThrow(token);
var task3 = DoWorkWithCancelCheck(token);

await Task.Delay(2000);

// cancel() stops the timer (if set) and then sets the state to NotCanceledState
// this one has already timed out by now but you get the point
cancellationTokenSource.Cancel(); 
Console.WriteLine("token cancelled!");
Console.ReadKey();

async Task DoWork(CancellationToken cancellationToken)
{
    // this workload never ends because we don't check the cancellationToken
    while (true)
    {
        await Task.Delay(250);
        Console.WriteLine("task 1");
    }
}

async Task DoWorkWithCancelThrow(CancellationToken cancellationToken)
{
    try
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(250);
            Console.WriteLine("task 2");
        }
    }
    catch (OperationCanceledException)
    {
        await Console.Out.WriteLineAsync("task 2 threw a cancellation exception");
    }
    
}

async Task DoWorkWithCancelCheck(CancellationToken cancellationToken)
{
    // exclict check
    while (!cancellationToken.IsCancellationRequested)
    {
        await Task.Delay(250);
        Console.WriteLine("task 3");
    }
    await Console.Out.WriteLineAsync("cancelled task 3 gracefully");
}
