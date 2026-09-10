namespace EventContracts;

/// <summary>
/// Carries an opaque, case-sensitive tenant identity.
/// </summary>
public sealed record TenantId
{
    /// <summary>
    /// Gets the tenant identity exactly as supplied.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a tenant identity without normalizing it.
    /// </summary>
    /// <param name="value">The opaque tenant identity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is empty or entirely whitespace.</exception>
    public TenantId(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A tenant ID cannot be empty or whitespace.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Returns the tenant identity exactly as supplied.
    /// </summary>
    /// <returns>The stored tenant identity.</returns>
    public override string ToString() => Value;
}
