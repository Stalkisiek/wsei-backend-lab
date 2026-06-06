using Xunit;
using System;
using CoreApp.Models;

namespace UnitTest;

public class PeselTests
{
    private static string GenerateValidPesel(string baseNumber)
    {
        if (baseNumber.Length != 10 || !baseNumber.All(char.IsDigit))
            throw new ArgumentException("Base PESEL must be 10 digits");
            
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += (baseNumber[i] - '0') * weights[i];
        }
        int checksum = (10 - (sum % 10)) % 10;
        return baseNumber + checksum;
    }

    private static string ValidMalePesel => GenerateValidPesel("9001011234");
    private static string ValidFemalePesel => GenerateValidPesel("9202020000");

    #region Validation Tests

    [Fact]
    public void From_WithValidPesel_ShouldCreateInstance()
    {
        var validPesel = ValidMalePesel;

        var pesel = Pesel.From(validPesel);

        Assert.NotNull(pesel);
        Assert.Equal(validPesel, pesel.ToString());
    }

    [Fact]
    public void From_WithValidPeselAndSpaces_ShouldTrimAndCreateInstance()
    {
        var validPesel = ValidMalePesel;

        var pesel = Pesel.From("  " + validPesel + "  ");

        Assert.NotNull(pesel);
        Assert.Equal(validPesel, pesel.ToString());
    }

    [Fact]
    public void From_WithEmptyString_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From(""));
        Assert.Contains("PESEL cannot be empty", ex.Message);
    }

    [Fact]
    public void From_WithNull_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From(null!));
        Assert.Contains("PESEL cannot be empty", ex.Message);
    }

    [Fact]
    public void From_WithTooShortPesel_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From("1234567890"));
        Assert.Contains("must be exactly 11 digits long", ex.Message);
    }

    [Fact]
    public void From_WithTooLongPesel_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From("123456789012"));
        Assert.Contains("must be exactly 11 digits long", ex.Message);
    }

    [Fact]
    public void From_WithNonDigitCharacters_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From("9001012234A"));
        Assert.Contains("must contain only digits", ex.Message);
    }

    [Fact]
    public void From_WithInvalidChecksum_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Pesel.From("90010122340"));
        Assert.Contains("checksum validation failed", ex.Message);
    }

    #endregion

    #region Valid PESEL Tests

    [Fact]
    public void From_WithValidPesels_ShouldSucceedForMultipleValues()
    {
        var pesels = new[] { ValidMalePesel, ValidFemalePesel };

        foreach (var peselValue in pesels)
        {
            var pesel = Pesel.From(peselValue);
            Assert.NotNull(pesel);
        }
    }

    #endregion

    #region Birth Date Extraction Tests

    [Fact]
    public void GetBirthDate_With1990sPesel_ShouldReturnCorrectDate()
    {
        var pesel = Pesel.From(GenerateValidPesel("9001010000"));

        var birthDate = pesel.GetBirthDate();

        Assert.Equal(new DateTime(1990, 1, 1), birthDate);
    }

    [Fact]
    public void GetBirthDate_WithValidPesel_ShouldExtractBirthDate()
    {
        var pesel = Pesel.From(ValidMalePesel);

        var birthDate = pesel.GetBirthDate();

        Assert.True(birthDate.Year >= 1800 && birthDate.Year <= DateTime.Now.Year);
    }

    #endregion

    #region Gender Extraction Tests

    [Fact]
    public void GetGender_WithMalePesel_ShouldReturnM()
    {
        var pesel = Pesel.From(GenerateValidPesel("9001019001"));

        var gender = pesel.GetGender();

        Assert.Equal('M', gender);
    }

    [Fact]
    public void GetGender_WithFemalePesel_ShouldReturnF()
    {
        var pesel = Pesel.From(GenerateValidPesel("9001010000"));

        var gender = pesel.GetGender();

        Assert.Equal('F', gender);
    }

    #endregion

    #region ValueObject Tests

    [Fact]
    public void TwoEqualPesels_ShouldBeEqual()
    {
        var peselValue = ValidMalePesel;
        var pesel1 = Pesel.From(peselValue);
        var pesel2 = Pesel.From(peselValue);

        Assert.Equal(pesel1, pesel2);
        Assert.True(pesel1 == pesel2);
        Assert.False(pesel1 != pesel2);
    }

    [Fact]
    public void TwoDifferentPesels_ShouldNotBeEqual()
    {
        var pesel1 = Pesel.From(ValidMalePesel);
        var pesel2 = Pesel.From(ValidFemalePesel);

        Assert.NotEqual(pesel1, pesel2);
        Assert.True(pesel1 != pesel2);
        Assert.False(pesel1 == pesel2);
    }

    [Fact]
    public void PeselGetHashCode_ShouldReturnConsistentValue()
    {
        var peselValue = ValidMalePesel;
        var pesel = Pesel.From(peselValue);
        var peselCopy = Pesel.From(peselValue);

        var hash1 = pesel.GetHashCode();
        var hash2 = peselCopy.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void PeselImplicitConversionToString_ShouldReturnValue()
    {
        var peselValue = ValidMalePesel;
        var pesel = Pesel.From(peselValue);

        string peselString = pesel;

        Assert.Equal(peselValue, peselString);
    }

    #endregion

    #region TryFrom and FromOrNull Tests

    [Fact]
    public void TryFrom_WithValidPesel_ShouldReturnTrue()
    {
        var peselValue = ValidMalePesel;

        var result = Pesel.TryFrom(peselValue, out var pesel);

        Assert.True(result);
        Assert.NotNull(pesel);
        Assert.Equal(peselValue, pesel.ToString());
    }

    [Fact]
    public void TryFrom_WithInvalidPesel_ShouldReturnFalse()
    {
        var result = Pesel.TryFrom("12345678901", out var pesel);

        Assert.False(result);
        Assert.Null(pesel);
    }

    [Fact]
    public void FromOrNull_WithValidPesel_ShouldReturnPesel()
    {
        var peselValue = ValidMalePesel;

        var pesel = Pesel.FromOrNull(peselValue);

        Assert.NotNull(pesel);
        Assert.Equal(peselValue, pesel.ToString());
    }

    [Fact]
    public void FromOrNull_WithInvalidPesel_ShouldReturnNull()
    {
        var pesel = Pesel.FromOrNull("12345678901");

        Assert.Null(pesel);
    }

    [Fact]
    public void FromOrNull_WithNull_ShouldReturnNull()
    {
        var pesel = Pesel.FromOrNull(null);

        Assert.Null(pesel);
    }

    [Fact]
    public void FromOrNull_WithEmptyString_ShouldReturnNull()
    {
        var pesel = Pesel.FromOrNull("");

        Assert.Null(pesel);
    }

    #endregion

    #region Format Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("12345678901234")]
    [InlineData("1234567890a")]
    [InlineData("! \" # $ % ^ &")]
    public void From_WithInvalidFormats_ShouldThrow(string invalidPesel)
    {
        Assert.Throws<ArgumentException>(() => Pesel.From(invalidPesel));
    }

    #endregion
}




