using System.Reflection;

namespace EventContracts.Tests;

public sealed class EventEnvelopeTests
{
    [Fact]
    public void Construction_preserves_exact_metadata_and_reference_payload()
    {
        var metadata = EventMetadataTests.CreateMetadata();
        var payload = new TestPayload(42);

        var envelope = new EventEnvelope<TestPayload>(metadata, payload);

        Assert.Same(metadata, envelope.Metadata);
        Assert.Same(payload, envelope.Payload);
    }

    [Fact]
    public void Construction_accepts_value_payload()
    {
        var envelope = new EventEnvelope<int>(EventMetadataTests.CreateMetadata(), 42);

        Assert.Equal(42, envelope.Payload);
    }

    [Fact]
    public void Construction_rejects_null_metadata_or_reference_payload()
    {
        var metadata = EventMetadataTests.CreateMetadata();

        Assert.Throws<ArgumentNullException>(() => new EventEnvelope<TestPayload>(null!, new TestPayload(1)));
        Assert.Throws<ArgumentNullException>(() => new EventEnvelope<TestPayload>(metadata, null!));
    }

    [Fact]
    public void Create_preserves_payload_and_every_caller_owned_metadata_value()
    {
        var payload = new TestPayload(42);
        var eventType = new EventTypeName("wallet.funds.reserved");
        var schemaVersion = new SchemaVersion(3);
        var occurredAt = new DateTimeOffset(2026, 4, 5, 6, 7, 8, TimeSpan.FromHours(5.5));
        var correlationId = new CorrelationId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var causationId = new CausationId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var tenantId = new TenantId("tenant-42");
        var traceContext = new TraceContext("00-0123456789abcdef0123456789abcdef-0123456789abcdef-01", "vendor=value");

        var envelope = EventEnvelope<TestPayload>.Create(
            payload,
            eventType,
            schemaVersion,
            occurredAt,
            correlationId,
            causationId,
            tenantId,
            traceContext);

        Assert.Same(payload, envelope.Payload);
        Assert.NotEqual(Guid.Empty, envelope.Metadata.EventId.Value);
        Assert.Same(eventType, envelope.Metadata.EventType);
        Assert.Equal(schemaVersion, envelope.Metadata.SchemaVersion);
        Assert.Equal(occurredAt, envelope.Metadata.OccurredAt);
        Assert.Equal(occurredAt.Offset, envelope.Metadata.OccurredAt.Offset);
        Assert.Equal(correlationId, envelope.Metadata.CorrelationId);
        Assert.Equal(causationId, envelope.Metadata.CausationId);
        Assert.Same(tenantId, envelope.Metadata.TenantId);
        Assert.Same(traceContext, envelope.Metadata.TraceContext);
    }

    [Fact]
    public void Create_leaves_all_optional_context_absent_when_not_supplied()
    {
        var envelope = EventEnvelope<int>.Create(
            42,
            new EventTypeName("wallet.balance.changed"),
            new SchemaVersion(1),
            new DateTimeOffset(2026, 4, 5, 6, 7, 8, TimeSpan.FromHours(5.5)));

        Assert.Null(envelope.Metadata.CorrelationId);
        Assert.Null(envelope.Metadata.CausationId);
        Assert.Null(envelope.Metadata.TenantId);
        Assert.Null(envelope.Metadata.TraceContext);
    }

    [Fact]
    public void Separate_Create_calls_generate_distinct_event_identifiers()
    {
        var eventType = new EventTypeName("wallet.balance.changed");
        var schemaVersion = new SchemaVersion(1);
        var occurredAt = new DateTimeOffset(2026, 4, 5, 6, 7, 8, TimeSpan.FromHours(5.5));

        var first = EventEnvelope<int>.Create(1, eventType, schemaVersion, occurredAt);
        var second = EventEnvelope<int>.Create(1, eventType, schemaVersion, occurredAt);

        Assert.NotEqual(Guid.Empty, first.Metadata.EventId.Value);
        Assert.NotEqual(Guid.Empty, second.Metadata.EventId.Value);
        Assert.NotEqual(first.Metadata.EventId, second.Metadata.EventId);
    }

    [Fact]
    public void Create_does_not_invent_correlation_when_caller_propagates_one()
    {
        var correlationId = new CorrelationId(Guid.Parse("11111111-1111-1111-1111-111111111111"));

        var envelope = EventEnvelope<int>.Create(
            42,
            new EventTypeName("wallet.balance.changed"),
            new SchemaVersion(1),
            new DateTimeOffset(2026, 4, 5, 6, 7, 8, TimeSpan.FromHours(5.5)),
            correlationId);

        Assert.Equal(correlationId, envelope.Metadata.CorrelationId);
    }

    [Fact]
    public void Equality_uses_metadata_value_equality_and_default_payload_equality()
    {
        var firstMetadata = EventMetadataTests.CreateMetadata();
        var equalMetadata = EventMetadataTests.CreateMetadata();
        var sharedPayload = new[] { 1, 2, 3 };

        Assert.Equal(
            new EventEnvelope<int[]>(firstMetadata, sharedPayload),
            new EventEnvelope<int[]>(equalMetadata, sharedPayload));
        Assert.NotEqual(
            new EventEnvelope<int[]>(firstMetadata, new[] { 1, 2, 3 }),
            new EventEnvelope<int[]>(equalMetadata, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void Envelope_and_metadata_properties_are_get_only()
    {
        Assert.All(typeof(EventEnvelope<>).GetProperties(BindingFlags.Public | BindingFlags.Instance), property => Assert.False(property.CanWrite));
        Assert.All(typeof(EventMetadata).GetProperties(BindingFlags.Public | BindingFlags.Instance), property => Assert.False(property.CanWrite));
    }

    private sealed record TestPayload(int Amount);
}
