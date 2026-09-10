using System.Globalization;

namespace EventContracts;

/// <summary>
/// Identifies one event occurrence.
/// </summary>
public readonly record struct EventId
{
    /// <summary>
    /// Gets the underlying non-empty GUID.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes an event ID.
    /// </summary>
    /// <param name="value">The non-empty GUID value.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
    public EventId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("An event ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Generates a new event ID.
    /// </summary>
    /// <returns>A new non-empty event ID.</returns>
    public static EventId New() => new(Guid.NewGuid());

    /// <summary>
    /// Parses a canonical hyphenated GUID into an event ID.
    /// </summary>
    /// <param name="value">A canonical <c>D</c>-format GUID.</param>
    /// <returns>The parsed event ID.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="value"/> is not canonical or represents the empty GUID.</exception>
    public static EventId Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return TryParse(value, out var result)
            ? result
            : throw new FormatException("The value is not a canonical non-empty event ID.");
    }

    /// <summary>
    /// Attempts to parse a canonical hyphenated GUID into an event ID.
    /// </summary>
    /// <param name="value">A canonical <c>D</c>-format GUID.</param>
    /// <param name="result">The parsed ID, or the default value when parsing fails.</param>
    /// <returns><see langword="true"/> when parsing succeeds; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out EventId result)
    {
        result = default;
        if (value is null || value.Length != 36 || !Guid.TryParseExact(value, "D", out var parsed) || parsed == Guid.Empty)
        {
            return false;
        }

        result = new EventId(parsed);
        return true;
    }

    /// <summary>
    /// Returns the lowercase canonical hyphenated representation.
    /// </summary>
    /// <returns>The lowercase <c>D</c>-format GUID.</returns>
    public override string ToString() => Value.ToString("D", CultureInfo.InvariantCulture);
}
