namespace EventContracts.Tests;

public sealed class TraceContextTests
{
    private const string ValidTraceParent = "00-0123456789abcdef0123456789abcdef-0123456789abcdef-01";

    [Fact]
    public void Valid_trace_context_is_preserved_exactly()
    {
        const string traceState = "vendor=value,second=other value";

        var context = new TraceContext(ValidTraceParent, traceState);

        Assert.Equal(ValidTraceParent, context.TraceParent);
        Assert.Equal(traceState, context.TraceState);
    }

    [Fact]
    public void Null_trace_state_is_accepted()
    {
        Assert.Null(new TraceContext(ValidTraceParent).TraceState);
    }

    [Fact]
    public void Printable_trace_state_boundaries_are_accepted()
    {
        Assert.Equal("!", new TraceContext(ValidTraceParent, "!").TraceState);
        var maximum = "x" + new string(' ', 511);
        Assert.Equal(maximum, new TraceContext(ValidTraceParent, maximum).TraceState);
        Assert.Equal("~", new TraceContext(ValidTraceParent, "~").TraceState);
    }

    [Fact]
    public void Null_trace_parent_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => new TraceContext(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("00-0123456789ABCDEF0123456789ABCDEF-0123456789abcdef-01")]
    [InlineData("00-0123456789abcdef0123456789abcdef_0123456789abcdef-01")]
    [InlineData("00-0123456789abcdef0123456789abcdef-0123456789abcdeg-01")]
    [InlineData("00-0123456789abcdef0123456789abcdef-0123456789abcdef-0")]
    [InlineData("01-0123456789abcdef0123456789abcdef-0123456789abcdef-01")]
    [InlineData("00-00000000000000000000000000000000-0123456789abcdef-01")]
    [InlineData("00-0123456789abcdef0123456789abcdef-0000000000000000-01")]
    [InlineData(" 00-0123456789abcdef0123456789abcdef-0123456789abcdef-01")]
    [InlineData("00-0123456789abcdef0123456789abcdef-0123456789abcdef-01 ")]
    public void Invalid_trace_parent_is_rejected(string traceParent)
    {
        Assert.Throws<ArgumentException>(() => new TraceContext(traceParent));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("line\nbreak")]
    [InlineData("line\rbreak")]
    [InlineData("control\tcharacter")]
    [InlineData("non-ascii-é")]
    public void Invalid_trace_state_is_rejected(string traceState)
    {
        Assert.Throws<ArgumentException>(() => new TraceContext(ValidTraceParent, traceState));
    }

    [Fact]
    public void Trace_state_longer_than_512_characters_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new TraceContext(ValidTraceParent, new string('x', 513)));
    }

    [Fact]
    public void Equality_is_value_based_across_both_fields()
    {
        var first = new TraceContext(ValidTraceParent, "vendor=value");
        var equal = new TraceContext(ValidTraceParent, "vendor=value");
        var different = new TraceContext(ValidTraceParent, "vendor=other");

        Assert.Equal(first, equal);
        Assert.NotEqual(first, different);
    }
}
