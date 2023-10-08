namespace Delegates;

// Using static classes for convenience but this can be done
// with object instances either constructed manually or injected

internal delegate void MoveForward();
internal delegate void Eat();
internal delegate void SwingTail();

internal class Horse
{
    private readonly MoveForward moveForward = FourLegs.MoveForward;
    private readonly SwingTail swingTail = MediumLengthTail.Swish;

    public void Go()
    {
        moveForward();
        swingTail();
    }
}

internal class Human
{
    private readonly MoveForward moveForward = TwoLegs.MoveForward;
    public void Go()
    {
        moveForward();
    }
}

internal class MediumLengthTail
{
    internal static void Swish()
    {
        Console.WriteLine("Tail goes swish");
    }
}

internal class FourLegs
{
    internal static void MoveForward()
    {
        Console.WriteLine("Trotting on four legs");
    }
}

internal partial class TwoLegs
{
    internal static void MoveForward()
    {
        Console.WriteLine("Walking on two legs");
    }
}