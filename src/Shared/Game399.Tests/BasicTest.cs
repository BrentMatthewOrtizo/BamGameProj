using Xunit;
//using Game399.Shared;

namespace Game399.Tests;

public class BasicTest
{
    [Fact]
    public void GetFullName_ReturnsConcatenatedName()
    {
        // Arrange
        string expected = "John Doe";
        string firstName = "John";
        string lastName = "Doe";

        // Act
        string actual = $"{firstName} {lastName}";

        // Assert
        Assert.Equal(expected, actual);
    }
}
