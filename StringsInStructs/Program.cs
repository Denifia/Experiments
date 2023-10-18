using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// The benchmarks show that I don't understand anything! lol.
// The 'ShouldBeLowerMemoryStruct' have more gen0 bytes to collect and allocate 2x kb

var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

[MemoryDiagnoser]
[MarkdownExporter]
public class Tester
{
    [Params(1, 10, 100)]
    public int TokensCreated { get; set; }

    [Benchmark]
    public void UseShouldBeLowerMemoryStruct()
    {
        string s;
        for (int i = 0; i < TokensCreated; i++)
        {
            s = new ShouldBeLowerMemoryStruct(Guid.NewGuid()).ToString();
        }
    }

    [Benchmark]
    public void UseShouldBeHigherMemoryStruct()
    {
        string s;
        for (int i = 0; i < TokensCreated; i++)
        {
            s = new ShouldBeHigherMemoryStruct(Guid.NewGuid()).ToString();
        }
    }
}

readonly struct ShouldBeLowerMemoryStruct
{
    private static readonly string _prefix = "tkn-";
    private readonly char[] _chars = new char[40];
    private readonly bool _hasValue = true;

    public ShouldBeLowerMemoryStruct(Guid guid)
    {
        // Copy _prefix onto the start of _chars
        _prefix.AsSpan().CopyTo(_chars.AsSpan(0, 4));

        // Format the guid as a string directly onto _chars 
        _ = guid.TryFormat(_chars.AsSpan(4), out int _);
    }

    public override string ToString() => new(_chars.AsSpan());

    public readonly bool IsNone => !_hasValue;
}

readonly struct ShouldBeHigherMemoryStruct
{
    private static readonly string _prefix = "tkn-";
    private readonly string _token;
    private readonly bool _hasValue = true;

    public ShouldBeHigherMemoryStruct(Guid guid)
    {
        _token = $"{_prefix}{guid}";
    }

    public override string ToString() => _token;

    public readonly bool IsNone => !_hasValue;
}
