using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddOptions<PersonOptions>()
    .Bind(builder.Configuration.GetSection(PersonOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

//var options = host.Services.GetRequiredService<IOptions<PersonOptions>>();
//logger.LogInformation(JsonSerializer.Serialize(options));

await host.StopAsync();


class PersonOptions()
{
    public const string Section = nameof(PersonOptions);

    [RegularExpression(@"^[a-z]{1,40}$")]
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
