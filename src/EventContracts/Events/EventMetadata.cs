namespace EventContracts;

/// <summary>
/// Describes the stable identity, contract, occurrence, and optional workflow context of an event.
/// </summary>
public sealed record EventMetadata
{
    /// <summary>
    /// Gets the unique identity of this event occurrence.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// Gets the canonical event contract name.
    /// </summary>
    public EventTypeName EventType { get; }

    /// <summary>
    /// Gets the revision of the payload schema.
    /// </summary>
    public SchemaVersion SchemaVersion { get; }

    /// <summary>
    /// Gets when the represented business fact occurred, preserving the supplied offset.
    /// </summary>
    public DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets the optional identity that groups related business work.
    /// </summary>
    public CorrelationId? CorrelationId { get; }

    /// <summary>
    /// Gets the optional identity of the event or action that caused this event.
    /// </summary>
    public CausationId? CausationId { get; }

    /// <summary>
    /// Gets the optional opaque tenant context.
    /// </summary>
    public TenantId? TenantId { get; }

    /// <summary>
    /// Gets the optional distributed trace context, which is independent from correlation.
    /// </summary>
    public TraceContext? TraceContext { get; }

    /// <summary>
    /// Initializes event metadata while preserving every supplied value exactly.
    /// </summary>
    /// <param name="eventId">The unique identity of this event occurrence.</param>
    /// <param name="eventType">The canonical event contract name.</param>
    /// <param name="schemaVersion">The payload schema revision.</param>
    /// <param name="occurredAt">The time the represented business fact occurred.</param>
    /// <param name="correlationId">The optional identity grouping related business work.</param>
    /// <param name="causationId">The optional identity of the event or action that caused this event.</param>
    /// <param name="tenantId">The optional opaque tenant context.</param>
    /// <param name="traceContext">The optional distributed trace context.</param>
    /// <exception cref="ArgumentNullException"><paramref name="eventType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A required value or a present optional identifier has its invalid default value.</exception>
    public EventMetadata(
        EventId eventId,
        EventTypeName eventType,
        SchemaVersion schemaVersion,
        DateTimeOffset occurredAt,
        CorrelationId? correlationId = null,
        CausationId? causationId = null,
        TenantId? tenantId = null,
        TraceContext? traceContext = null)
    {
        if (eventId.Value == Guid.Empty)
        {
            throw new ArgumentException("The event ID must not be the default value.", nameof(eventId));
        }

        ArgumentNullException.ThrowIfNull(eventType);

        if (schemaVersion.Value <= 0)
        {
            throw new ArgumentException("The schema version must not be the default value.", nameof(schemaVersion));
        }

        if (occurredAt == default)
        {
            throw new ArgumentException("The occurrence time must not be the default value.", nameof(occurredAt));
        }

        if (correlationId is { Value: var correlationValue } && correlationValue == Guid.Empty)
        {
            throw new ArgumentException("A present correlation ID must not be the default value.", nameof(correlationId));
        }

        if (causationId is { Value: var causationValue } && causationValue == Guid.Empty)
        {
            throw new ArgumentException("A present causation ID must not be the default value.", nameof(causationId));
        }

        EventId = eventId;
        EventType = eventType;
        SchemaVersion = schemaVersion;
        OccurredAt = occurredAt;
        CorrelationId = correlationId;
        CausationId = causationId;
        TenantId = tenantId;
        TraceContext = traceContext;
    }

    /// <summary>
    /// Determines whether all metadata values, including the supplied occurrence offset, are equal.
    /// </summary>
    /// <param name="other">The metadata to compare.</param>
    /// <returns><see langword="true"/> when every stored value is equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(EventMetadata? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is not null &&
            EventId == other.EventId &&
            EventType == other.EventType &&
            SchemaVersion == other.SchemaVersion &&
            OccurredAt.Ticks == other.OccurredAt.Ticks &&
            OccurredAt.Offset == other.OccurredAt.Offset &&
            CorrelationId == other.CorrelationId &&
            CausationId == other.CausationId &&
            TenantId == other.TenantId &&
            TraceContext == other.TraceContext;
    }

    /// <summary>
    /// Returns a hash code that includes every stored metadata value and the supplied occurrence offset.
    /// </summary>
    /// <returns>A hash code for this metadata.</returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(EventId);
        hashCode.Add(EventType);
        hashCode.Add(SchemaVersion);
        hashCode.Add(OccurredAt.Ticks);
        hashCode.Add(OccurredAt.Offset);
        hashCode.Add(CorrelationId);
        hashCode.Add(CausationId);
        hashCode.Add(TenantId);
        hashCode.Add(TraceContext);
        return hashCode.ToHashCode();
    }
}
