namespace EventContracts.Tests;

public sealed class EventMetadataTests
{
    private static readonly EventId ValidEventId = new(Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"));
    private static readonly EventTypeName ValidEventType = new("wallet.balance.changed");
    private static readonly SchemaVersion ValidSchemaVersion = new(1);
    private static readonly DateTimeOffset ValidOccurredAt = new(2026, 2, 3, 4, 5, 6, TimeSpan.FromHours(5.5));

    [Fact]
    public void Construction_preserves_all_values_and_original_offset()
    {
        var correlationId = new CorrelationId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var causationId = new CausationId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var tenantId = new TenantId(" tenant-42 ");
        var traceContext = new TraceContext("00-0123456789abcdef0123456789abcdef-0123456789abcdef-01", "vendor=value");

        var metadata = new EventMetadata(
            ValidEventId,
            ValidEventType,
            ValidSchemaVersion,
            ValidOccurredAt,
            correlationId,
            causationId,
            tenantId,
            traceContext);

        Assert.Equal(ValidEventId, metadata.EventId);
        Assert.Same(ValidEventType, metadata.EventType);
        Assert.Equal(ValidSchemaVersion, metadata.SchemaVersion);
        Assert.Equal(ValidOccurredAt, metadata.OccurredAt);
        Assert.Equal(ValidOccurredAt.Offset, metadata.OccurredAt.Offset);
        Assert.Equal(correlationId, metadata.CorrelationId);
        Assert.Equal(causationId, metadata.CausationId);
        Assert.Same(tenantId, metadata.TenantId);
        Assert.Same(traceContext, metadata.TraceContext);
    }

    [Fact]
    public void Optional_context_can_be_independently_absent()
    {
        var metadata = CreateMetadata();

        Assert.Null(metadata.CorrelationId);
        Assert.Null(metadata.CausationId);
        Assert.Null(metadata.TenantId);
        Assert.Null(metadata.TraceContext);
    }

    [Fact]
    public void Required_values_are_rejected_when_missing_or_default()
    {
        Assert.Throws<ArgumentException>(() => CreateMetadata(eventId: default(EventId)));
        Assert.Throws<ArgumentNullException>(() => new EventMetadata(ValidEventId, null!, ValidSchemaVersion, ValidOccurredAt));
        Assert.Throws<ArgumentException>(() => CreateMetadata(schemaVersion: default(SchemaVersion)));
        Assert.Throws<ArgumentException>(() => CreateMetadata(occurredAt: default(DateTimeOffset)));
    }

    [Fact]
    public void Present_default_workflow_identifiers_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => CreateMetadata(correlationId: default(CorrelationId)));
        Assert.Throws<ArgumentException>(() => CreateMetadata(causationId: default(CausationId)));
    }

    [Fact]
    public void OccurredAt_is_not_derived_or_substituted()
    {
        var historical = new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.FromHours(-7));
        var metadata = CreateMetadata(occurredAt: historical);

        Assert.Equal(historical, metadata.OccurredAt);
        Assert.Equal(historical.Offset, metadata.OccurredAt.Offset);
    }

    [Fact]
    public void Equality_includes_every_member()
    {
        var first = CreateMetadata();
        var equal = CreateMetadata();

        Assert.Equal(first, equal);
        Assert.NotEqual(first, CreateMetadata(eventId: new EventId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"))));
        Assert.NotEqual(first, CreateMetadata(eventType: new EventTypeName("wallet.balance.decreased")));
        Assert.NotEqual(first, CreateMetadata(schemaVersion: new SchemaVersion(2)));
        Assert.NotEqual(first, CreateMetadata(occurredAt: ValidOccurredAt.AddTicks(1)));
        Assert.NotEqual(first, CreateMetadata(correlationId: new CorrelationId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"))));
        Assert.NotEqual(first, CreateMetadata(causationId: new CausationId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"))));
        Assert.NotEqual(first, CreateMetadata(tenantId: new TenantId("tenant")));
        Assert.NotEqual(first, CreateMetadata(traceContext: new TraceContext("00-0123456789abcdef0123456789abcdef-0123456789abcdef-01")));
    }

    [Fact]
    public void Equality_includes_the_supplied_occurrence_offset()
    {
        var first = CreateMetadata();
        var sameInstantWithDifferentOffset = CreateMetadata(occurredAt: ValidOccurredAt.ToOffset(TimeSpan.Zero));

        Assert.NotEqual(ValidOccurredAt.Offset, sameInstantWithDifferentOffset.OccurredAt.Offset);
        Assert.NotEqual(first, sameInstantWithDifferentOffset);
    }

    internal static EventMetadata CreateMetadata(
        EventId? eventId = null,
        EventTypeName? eventType = null,
        SchemaVersion? schemaVersion = null,
        DateTimeOffset? occurredAt = null,
        CorrelationId? correlationId = null,
        CausationId? causationId = null,
        TenantId? tenantId = null,
        TraceContext? traceContext = null) =>
        new(
            eventId ?? ValidEventId,
            eventType ?? ValidEventType,
            schemaVersion ?? ValidSchemaVersion,
            occurredAt ?? ValidOccurredAt,
            correlationId,
            causationId,
            tenantId,
            traceContext);
}
