using System.Globalization;

namespace EventContracts.Tests;

public sealed class SchemaVersionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public void Positive_values_are_preserved(int value)
    {
        Assert.Equal(value, new SchemaVersion(value).Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Non_positive_values_are_rejected(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SchemaVersion(value));
    }

    [Fact]
    public void Equality_and_hash_codes_are_value_based()
    {
        var first = new SchemaVersion(2);
        var second = new SchemaVersion(2);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ToString_uses_invariant_decimal_formatting()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
            Assert.Equal("1234567", new SchemaVersion(1_234_567).ToString());
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
