using System;
using Xunit;
using Moq;

public class SimpleTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        int a = 1;
        int b = 1;
        
        // Act
        int result = a + b;
        
        // Assert
        Assert.Equal(2, result);
        Console.WriteLine("Test passed successfully!");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Running test manually:");
        var test = new SimpleTests();
        test.Test1();
        Console.WriteLine("All tests completed!");
    }
}
