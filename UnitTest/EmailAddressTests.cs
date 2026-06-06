using CoreApp.Models;
using Xunit;

namespace UnitTest;

public class EmailAddressTests
{
    [Fact]
    public void From_WithValidEmail_ShouldParseUserAndDomain()
    {
        var email = EmailAddress.From("Jan.Kowalski@Example.Local");

        Assert.Equal("jan.kowalski@example.local", email.ToString());
        Assert.Equal("jan.kowalski", email.User);
        Assert.Equal("example.local", email.Domain);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("abc")]
    [InlineData("abc@")]
    [InlineData("@domain.com")]
    [InlineData("a@b@c.com")]
    public void From_WithInvalidFormat_ShouldThrow(string value)
    {
        Assert.Throws<ArgumentException>(() => EmailAddress.From(value));
    }

    [Fact]
    public void TryFrom_WithValidEmail_ShouldReturnTrue()
    {
        var result = EmailAddress.TryFrom("user@domain.com", out var email);

        Assert.True(result);
        Assert.NotNull(email);
        Assert.Equal("user@domain.com", email!.Format());
    }
}

