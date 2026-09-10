using System.Globalization;

namespace EventContracts;

/// <summary>
/// Identifies a positive revision of an event payload schema.
/// </summary>
public readonly record struct SchemaVersion
{
    /// <summary>
    /// Gets the positive schema revision.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a schema version.
    /// </summary>
    /// <param name="value">A positive schema revision.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not positive.</exception>
    public SchemaVersion(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "A schema version must be positive.");
        }

        Value = value;
    }

    /// <summary>
    /// Returns the invariant decimal representation.
    /// </summary>
    /// <returns>The invariant decimal schema revision.</returns>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
