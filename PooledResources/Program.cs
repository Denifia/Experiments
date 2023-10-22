using System.Buffers;

var count = 10;

// Rent an array of ints that is AT LEAST the minimumLength.
// It can be bigger than what you wanted and most likely a power of 2.
var numbers = ArrayPool<int>.Shared.Rent(minimumLength: count);

Console.WriteLine($"Rented an int[] with {numbers.Length} size");

// A foreach here may iterate over more int that we're expecting
for (int i = 0; i < count; i++)
{
    // Using the shared random instance
    numbers[i] = Random.Shared.Next(minValue: 50, maxValue: 101);
}

// Get elements with index 2, 3, and 4
// The lower bounds is inclusive, the upper is exclusive
var someNumbers = numbers.AsSpan(2..5);

foreach (var number in someNumbers)
{
    Console.WriteLine(number);
}

// Gotta return what you rent
ArrayPool<int>.Shared.Return(numbers);
