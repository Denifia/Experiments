namespace Delegates;

// delegates defined in BetterExample.cs

internal interface IMove
{
    void Move();
}

internal class Dog : IMove
{
    private readonly MoveForward moveForward = FourLegs.MoveForward;
    private readonly SwingTail swingTail = MediumLengthTail.Swish;

    public void Move()
    {
        moveForward();
        swingTail();
    }
}

internal class Kangeroo : IMove
{
    private readonly MoveForward moveForward = TwoLegs.Jump;

    public void Move()
    {
        moveForward();
    }
}

internal class ShortTail
{
    internal static void Swish()
    {
        Console.WriteLine("Tail goes waggle waggle");
    }
}

internal partial class TwoLegs
{
    internal static void Jump()
    {
        Console.WriteLine("Two legged jump!");
    }
}