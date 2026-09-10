# EventContracts

EventContracts is a small .NET 8 library for transport-independent integration-event envelopes. V1 provides strongly typed event, correlation, and causation identifiers together with schema, tenant, trace, naming, and occurrence-time metadata. It does not depend on a broker, serializer, database, dependency-injection framework, or application domain.

## Create an envelope

```csharp
using EventContracts;

var correlationId = new CorrelationId(Guid.NewGuid());
var envelope = EventEnvelope<BalanceChanged>.Create(
    new BalanceChanged(25.00m),
    new EventTypeName("wallet.balance.changed"),
    new SchemaVersion(1),
    DateTimeOffset.UtcNow,
    correlationId: correlationId,
    tenantId: new TenantId("tenant-42"));
```

`OccurredAt` is the caller-supplied time when the business fact occurred. Correlation groups related business work, causation identifies the event or action that led to a new event, and W3C trace context remains separate from both.

See `samples/EventContracts.Sample` for an executable correlation and causation example. The complete V1 behavior and deferred work are documented in `docs/` in the source repository.
