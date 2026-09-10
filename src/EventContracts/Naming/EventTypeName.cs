namespace EventContracts;

/// <summary>
/// Carries a canonical, version-free integration-event contract name.
/// </summary>
public sealed record EventTypeName
{
    /// <summary>
    /// Gets the canonical name exactly as supplied.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a lowercase ASCII dot-separated event type name.
    /// </summary>
    /// <param name="value">A name containing at least two valid segments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not canonical or contains a schema-version segment.</exception>
    public EventTypeName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!IsValid(value))
        {
            throw new ArgumentException("An event type name must contain at least two lowercase ASCII dot-separated segments and no version segment.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Returns the canonical event type name.
    /// </summary>
    /// <returns>The stored event type name.</returns>
    public override string ToString() => Value;

    private static bool IsValid(string value)
    {
        var segmentStart = 0;
        var segmentCount = 0;

        for (var index = 0; index <= value.Length; index++)
        {
            if (index < value.Length && value[index] != '.')
            {
                continue;
            }

            var segmentLength = index - segmentStart;
            if (!IsValidSegment(value, segmentStart, segmentLength) || IsVersionSegment(value, segmentStart, segmentLength))
            {
                return false;
            }

            segmentCount++;
            segmentStart = index + 1;
        }

        return segmentCount >= 2;
    }

    private static bool IsValidSegment(string value, int start, int length)
    {
        if (length == 0 || value[start] is < 'a' or > 'z')
        {
            return false;
        }

        for (var index = start + 1; index < start + length; index++)
        {
            var character = value[index];
            if (!((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9')))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsVersionSegment(string value, int start, int length)
    {
        if (length < 2 || value[start] != 'v')
        {
            return false;
        }

        for (var index = start + 1; index < start + length; index++)
        {
            if (value[index] is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }
}
