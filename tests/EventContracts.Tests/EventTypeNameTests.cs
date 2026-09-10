namespace EventContracts.Tests;

public sealed class EventTypeNameTests
{
    [Theory]
    [InlineData("player.registered")]
    [InlineData("wallet.funds.reserved")]
    [InlineData("a.b")]
    [InlineData("a1.b2")]
    [InlineData("player.v")]
    public void Valid_names_are_preserved_exactly(string value)
    {
        var eventType = new EventTypeName(value);

        Assert.Equal(value, eventType.Value);
        Assert.Equal(value, eventType.ToString());
    }

    [Fact]
    public void Null_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => new EventTypeName(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("registered")]
    [InlineData(".player.registered")]
    [InlineData("player.registered.")]
    [InlineData("player..registered")]
    [InlineData("Player.registered")]
    [InlineData("player.Registered")]
    [InlineData("player. registered")]
    [InlineData("player.registered_event")]
    [InlineData("player.registered-event")]
    [InlineData("player.registered!")]
    [InlineData("player.régistered")]
    [InlineData("player.2registered")]
    [InlineData("player.registered.v2")]
    [InlineData("v1.player")]
    [InlineData("player.v123")]
    public void Invalid_names_are_rejected(string value)
    {
        Assert.Throws<ArgumentException>(() => new EventTypeName(value));
    }

    [Fact]
    public void Equality_is_ordinal_and_case_sensitive()
    {
        Assert.Equal(new EventTypeName("player.registered"), new EventTypeName("player.registered"));
        Assert.NotEqual(new EventTypeName("player.registered"), new EventTypeName("player.cancelled"));
    }
}
