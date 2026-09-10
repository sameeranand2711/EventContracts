namespace EventContracts.Tests;

public sealed class TenantIdTests
{
    [Theory]
    [InlineData("tenant-42")]
    [InlineData(" tenant-42 ")]
    [InlineData("A")]
    public void Valid_value_is_preserved_exactly(string value)
    {
        var tenantId = new TenantId(value);

        Assert.Equal(value, tenantId.Value);
        Assert.Equal(value, tenantId.ToString());
    }

    [Fact]
    public void Null_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => new TenantId(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    public void Empty_or_whitespace_only_value_is_rejected(string value)
    {
        Assert.Throws<ArgumentException>(() => new TenantId(value));
    }

    [Fact]
    public void Equality_is_ordinal_and_case_sensitive()
    {
        Assert.Equal(new TenantId("tenant"), new TenantId("tenant"));
        Assert.NotEqual(new TenantId("tenant"), new TenantId("Tenant"));
    }
}
