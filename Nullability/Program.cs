#nullable enable

var foo = new Foo(1, null!);
Console.WriteLine(foo.Name);

record Foo(int Id, string Name);
