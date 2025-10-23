using NUnit.Framework;

//using Game399.Shared;

namespace Game399.Tests;
[TestFixture]
public class BasicTest
{
    [Test]
    public void GetFullName_ReturnsConcatenatedName()
    {
        // Arrange
        string expected = "John Doe";
        string firstName = "John";
        string lastName = "Doe";

        // Act
        string actual = $"{firstName} {lastName}";

        // Assert
        Assert.That(expected, Is.EqualTo(actual));
    }
}
