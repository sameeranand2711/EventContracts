namespace EventContracts;

/// <summary>
/// Carries canonical W3C distributed trace context independently from business correlation.
/// </summary>
public sealed record TraceContext
{
    /// <summary>
    /// Gets the canonical lowercase W3C version <c>00</c> traceparent value.
    /// </summary>
    public string TraceParent { get; }

    /// <summary>
    /// Gets the optional tracestate exactly as supplied.
    /// </summary>
    public string? TraceState { get; }

    /// <summary>
    /// Initializes trace context.
    /// </summary>
    /// <param name="traceParent">A canonical lowercase W3C version <c>00</c> traceparent.</param>
    /// <param name="traceState">An optional printable-ASCII tracestate of at most 512 characters.</param>
    /// <exception cref="ArgumentNullException"><paramref name="traceParent"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A supplied value is invalid.</exception>
    public TraceContext(string traceParent, string? traceState = null)
    {
        ArgumentNullException.ThrowIfNull(traceParent);
        if (!IsValidTraceParent(traceParent))
        {
            throw new ArgumentException("The traceparent must be a canonical lowercase W3C version 00 value.", nameof(traceParent));
        }

        if (traceState is not null && !IsValidTraceState(traceState))
        {
            throw new ArgumentException("The tracestate must contain 1 to 512 printable ASCII characters and at least one non-space character.", nameof(traceState));
        }

        TraceParent = traceParent;
        TraceState = traceState;
    }

    private static bool IsValidTraceParent(string value)
    {
        if (value.Length != 55 ||
            value[0] != '0' ||
            value[1] != '0' ||
            value[2] != '-' ||
            value[35] != '-' ||
            value[52] != '-')
        {
            return false;
        }

        if (!ContainsOnlyLowerHex(value, 3, 32) ||
            !ContainsOnlyLowerHex(value, 36, 16) ||
            !ContainsOnlyLowerHex(value, 53, 2))
        {
            return false;
        }

        return !ContainsOnlyZeros(value, 3, 32) && !ContainsOnlyZeros(value, 36, 16);
    }

    private static bool IsValidTraceState(string value)
    {
        if (value.Length is < 1 or > 512)
        {
            return false;
        }

        var hasNonSpace = false;
        foreach (var character in value)
        {
            if (character is < '\u0020' or > '\u007e')
            {
                return false;
            }

            hasNonSpace |= character != ' ';
        }

        return hasNonSpace;
    }

    private static bool ContainsOnlyLowerHex(string value, int start, int length)
    {
        for (var index = start; index < start + length; index++)
        {
            var character = value[index];
            if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f')))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContainsOnlyZeros(string value, int start, int length)
    {
        for (var index = start; index < start + length; index++)
        {
            if (value[index] != '0')
            {
                return false;
            }
        }

        return true;
    }
}
