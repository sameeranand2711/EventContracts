using System.Globalization;

namespace EventContracts;

/// <summary>
/// Identifies the event or action that caused an event.
/// </summary>
public readonly record struct CausationId
{
    /// <summary>
    /// Gets the underlying non-empty GUID.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a causation ID.
    /// </summary>
    /// <param name="value">The non-empty GUID value.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
    public CausationId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A causation ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Parses a canonical hyphenated GUID into a causation ID.
    /// </summary>
    /// <param name="value">A canonical <c>D</c>-format GUID.</param>
    /// <returns>The parsed causation ID.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="value"/> is not canonical or represents the empty GUID.</exception>
    public static CausationId Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return TryParse(value, out var result)
            ? result
            : throw new FormatException("The value is not a canonical non-empty causation ID.");
    }

    /// <summary>
    /// Attempts to parse a canonical hyphenated GUID into a causation ID.
    /// </summary>
    /// <param name="value">A canonical <c>D</c>-format GUID.</param>
    /// <param name="result">The parsed ID, or the default value when parsing fails.</param>
    /// <returns><see langword="true"/> when parsing succeeds; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out CausationId result)
    {
        result = default;
        if (value is null || value.Length != 36 || !Guid.TryParseExact(value, "D", out var parsed) || parsed == Guid.Empty)
        {
            return false;
        }

        result = new CausationId(parsed);
        return true;
    }

    /// <summary>
    /// Returns the lowercase canonical hyphenated representation.
    /// </summary>
    /// <returns>The lowercase <c>D</c>-format GUID.</returns>
    public override string ToString() => Value.ToString("D", CultureInfo.InvariantCulture);
}
