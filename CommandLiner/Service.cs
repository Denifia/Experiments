using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace CommandLiner;

internal class Service
{
    public async Task DoTheThingAsync()
    {
        await AnsiConsole.Status()
            .StartAsync("Thinking...", async context =>
            {
                AnsiConsole.MarkupLine("Doing some work...");
                await Task.Delay(TimeSpan.FromSeconds(1));
                context.Status("Thinking some more...");
                context.Spinner(Spinner.Known.Aesthetic);
                context.SpinnerStyle(Style.Parse("green"));
                AnsiConsole.MarkupLine("Doing some more work...");
                await Task.Delay(TimeSpan.FromSeconds(2));
                AnsiConsole.MarkupLine("all done!!");
            });
    }
}
