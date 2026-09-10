using System.Reflection;

namespace EventContracts.Tests;

public sealed class IdentifierTests
{
    private static readonly Guid ValidGuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
    private const string CanonicalLower = "01234567-89ab-cdef-0123-456789abcdef";
    private const string CanonicalUpper = "01234567-89AB-CDEF-0123-456789ABCDEF";

    [Fact]
    public void EventId_construction_and_equality_are_value_based()
    {
        var first = new EventId(ValidGuid);
        var second = new EventId(ValidGuid);

        Assert.Equal(ValidGuid, first.Value);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Throws<ArgumentException>(() => new EventId(Guid.Empty));
    }

    [Fact]
    public void CorrelationId_construction_and_equality_are_value_based()
    {
        var first = new CorrelationId(ValidGuid);
        var second = new CorrelationId(ValidGuid);

        Assert.Equal(ValidGuid, first.Value);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Throws<ArgumentException>(() => new CorrelationId(Guid.Empty));
    }

    [Fact]
    public void CausationId_construction_and_equality_are_value_based()
    {
        var first = new CausationId(ValidGuid);
        var second = new CausationId(ValidGuid);

        Assert.Equal(ValidGuid, first.Value);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Throws<ArgumentException>(() => new CausationId(Guid.Empty));
    }

    [Theory]
    [InlineData(CanonicalLower)]
    [InlineData(CanonicalUpper)]
    public void EventId_parses_canonical_D_form(string text)
    {
        Assert.Equal(ValidGuid, EventId.Parse(text).Value);
        Assert.True(EventId.TryParse(text, out var result));
        Assert.Equal(ValidGuid, result.Value);
    }

    [Theory]
    [InlineData(CanonicalLower)]
    [InlineData(CanonicalUpper)]
    public void CorrelationId_parses_canonical_D_form(string text)
    {
        Assert.Equal(ValidGuid, CorrelationId.Parse(text).Value);
        Assert.True(CorrelationId.TryParse(text, out var result));
        Assert.Equal(ValidGuid, result.Value);
    }

    [Theory]
    [InlineData(CanonicalLower)]
    [InlineData(CanonicalUpper)]
    public void CausationId_parses_canonical_D_form(string text)
    {
        Assert.Equal(ValidGuid, CausationId.Parse(text).Value);
        Assert.True(CausationId.TryParse(text, out var result));
        Assert.Equal(ValidGuid, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")]
    [InlineData("0123456789abcdef0123456789abcdef")]
    [InlineData("{01234567-89ab-cdef-0123-456789abcdef}")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData(" 01234567-89ab-cdef-0123-456789abcdef")]
    public void EventId_rejects_invalid_parse_input(string text)
    {
        Assert.Throws<FormatException>(() => EventId.Parse(text));
        Assert.False(EventId.TryParse(text, out var result));
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")]
    [InlineData("0123456789abcdef0123456789abcdef")]
    [InlineData("{01234567-89ab-cdef-0123-456789abcdef}")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("01234567-89ab-cdef-0123-456789abcdef ")]
    public void CorrelationId_rejects_invalid_parse_input(string text)
    {
        Assert.Throws<FormatException>(() => CorrelationId.Parse(text));
        Assert.False(CorrelationId.TryParse(text, out var result));
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")]
    [InlineData("0123456789abcdef0123456789abcdef")]
    [InlineData("{01234567-89ab-cdef-0123-456789abcdef}")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData(" 01234567-89ab-cdef-0123-456789abcdef ")]
    public void CausationId_rejects_invalid_parse_input(string text)
    {
        Assert.Throws<FormatException>(() => CausationId.Parse(text));
        Assert.False(CausationId.TryParse(text, out var result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void Parse_and_TryParse_handle_null_as_designed()
    {
        Assert.Throws<ArgumentNullException>(() => EventId.Parse(null!));
        Assert.Throws<ArgumentNullException>(() => CorrelationId.Parse(null!));
        Assert.Throws<ArgumentNullException>(() => CausationId.Parse(null!));

        Assert.False(EventId.TryParse(null, out var eventId));
        Assert.False(CorrelationId.TryParse(null, out var correlationId));
        Assert.False(CausationId.TryParse(null, out var causationId));
        Assert.Equal(default, eventId);
        Assert.Equal(default, correlationId);
        Assert.Equal(default, causationId);
    }

    [Fact]
    public void Identifiers_format_as_lowercase_canonical_D_form()
    {
        Assert.Equal(CanonicalLower, new EventId(ValidGuid).ToString());
        Assert.Equal(CanonicalLower, new CorrelationId(ValidGuid).ToString());
        Assert.Equal(CanonicalLower, new CausationId(ValidGuid).ToString());
    }

    [Fact]
    public void Identifier_types_have_no_implicit_conversions()
    {
        var identifierTypes = new[] { typeof(EventId), typeof(CorrelationId), typeof(CausationId) };

        Assert.All(identifierTypes, type =>
            Assert.DoesNotContain(
                type.GetMethods(BindingFlags.Public | BindingFlags.Static),
                method => method.Name == "op_Implicit"));
    }

    [Fact]
    public void EventId_New_produces_non_empty_unique_values()
    {
        var identifiers = Enumerable.Range(0, 1_000).Select(_ => EventId.New()).ToArray();

        Assert.All(identifiers, identifier => Assert.NotEqual(Guid.Empty, identifier.Value));
        Assert.Equal(identifiers.Length, identifiers.Distinct().Count());
    }

    [Fact]
    public void Correlation_and_causation_identifiers_have_no_generation_API()
    {
        Assert.DoesNotContain(typeof(CorrelationId).GetMethods(BindingFlags.Public | BindingFlags.Static), method => method.Name == "New");
        Assert.DoesNotContain(typeof(CausationId).GetMethods(BindingFlags.Public | BindingFlags.Static), method => method.Name == "New");
    }
}
