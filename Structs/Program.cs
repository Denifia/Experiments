// Construction
using System;

internal class Program
{
    private static void Main(string[] _)
    {
        Console.WriteLine("- Construction -");
        Console.WriteLine("new() = {0}", new Token());
        Console.WriteLine("default(Token) = {0}", default(Token));
        Console.WriteLine("Token.None = {0}", Token.None);
        Console.WriteLine("Token.New = {0}", Token.New);
        Console.WriteLine("new(\"...\") = {0}", new Token(Guid.NewGuid().ToString()));
        Console.WriteLine("new(\"tkn-...\") = {0}", new Token(Token.TokenPrefix + Guid.NewGuid().ToString()));

        // Conversion
        Console.WriteLine("\n- Implicit conversion -");
        string s = Token.New;
        Console.WriteLine("implicit convert token -> string {0}", s);
        Token t = Guid.NewGuid().ToString();
        Console.WriteLine("implicit convert string -> token {0}", t);

        // HashCode
        Console.WriteLine("\n- HashCode -");
        var guid = Guid.NewGuid().ToString();
        Console.WriteLine("get hashcode with same guid as below = {0}", new Token(guid).GetHashCode());
        Console.WriteLine("get hashcode with same guid as above = {0}", new Token(guid).GetHashCode());
        Console.WriteLine("get hashcode with different guid = {0}", new Token(Guid.NewGuid().ToString()).GetHashCode());
        Console.WriteLine("get hashcode with no Value = {0}", Token.None.GetHashCode());

        // Equality
        Console.WriteLine("\n- Equality -");
        var left = Token.New;
        var copyOfLeft = left;
        var right = Token.New;
        Console.WriteLine("same == is {0}", left == copyOfLeft);
        Console.WriteLine("same != is {0}", left != copyOfLeft);
        Console.WriteLine("different == is {0}", left == right);
        Console.WriteLine("different != is {0}", left != right);
        object o = left;
        Console.WriteLine("boxed == is {0}", left == (Token)o);
        Console.WriteLine("boxed .Equals is {0}", left.Equals(o));

        // Equality + Conversion
        Console.WriteLine("\n- Equality with implicit conversion -");
        Token token = Token.New;
        string tokenAsString = token;
        Console.WriteLine("same == is {0}", token == tokenAsString);
        Console.WriteLine("same != is {0}", token != tokenAsString);
    }
}

public readonly struct Token
{
    public const string TokenPrefix = "tkn-";
    private readonly string _value;
    public bool IsNone { get => _value is null; }

#if NET6_0_OR_GREATER
    // Primary constructor
    // I wanted this but seems no point if I'm supporting earlier
    // .net versions. `new Token()` is equivilant to `Token.None`
    //private readonly string? _value = default;
    //public Token() : this(Guid.NewGuid().ToString()) { }
#endif

    public Token(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        if ((value.StartsWith(TokenPrefix) && !Guid.TryParse(value.Substring(4), out var _)) // In case of "tkn-GUID"
            || (!value.StartsWith(TokenPrefix) && !Guid.TryParse(value, out var _))) // In case of "GUID"
            throw new ArgumentException("Value is not a valid token string:", value);

        _value = value.StartsWith(TokenPrefix)
            ? value
            : $"{TokenPrefix}{value}";
    }

    // Helper statics
    public static Token None => default;
    public static Token New => new Token(Guid.NewGuid().ToString());

    // Helper methods
    public override string ToString() => IsNone ? "none" : _value;

    // Implicit conversion to/from string
    public static implicit operator string(Token token) => token.ToString();
    public static implicit operator Token(string token) => new Token(token);

    // Equality
    public override bool Equals(object obj) => obj is Token other && this.Equals(other);
    public bool Equals(Token other) => _value == other._value;
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public static bool operator ==(Token lhs, Token rhs) => lhs.Equals(rhs);
    public static bool operator !=(Token lhs, Token rhs) => !(lhs == rhs);
}
