using EventContracts;

var correlationId = new CorrelationId(Guid.NewGuid());
var tenantId = new TenantId("casino-demo");
var betPlacedAt = new DateTimeOffset(2026, 9, 10, 14, 30, 0, TimeSpan.FromHours(5.5));

var betPlaced = EventEnvelope<BetPlaced>.Create(
    new BetPlaced("bet-1001", "player-42", 25.00m),
    new EventTypeName("bet.placed"),
    new SchemaVersion(1),
    betPlacedAt,
    correlationId: correlationId,
    tenantId: tenantId);

var fundsReserved = EventEnvelope<FundsReserved>.Create(
    new FundsReserved("bet-1001", 25.00m),
    new EventTypeName("wallet.funds.reserved"),
    new SchemaVersion(1),
    betPlacedAt.AddMilliseconds(150),
    correlationId: correlationId,
    causationId: new CausationId(betPlaced.Metadata.EventId.Value),
    tenantId: tenantId);

Console.WriteLine(string.Concat("Bet placed: ", betPlaced.Metadata.EventId.ToString()));
Console.WriteLine(string.Concat("Funds reserved: ", fundsReserved.Metadata.EventId.ToString()));
Console.WriteLine(string.Concat("Shared correlation: ", fundsReserved.Metadata.CorrelationId?.ToString()));
Console.WriteLine(string.Concat("Caused by bet event: ", fundsReserved.Metadata.CausationId?.ToString()));

internal sealed record BetPlaced(string BetId, string PlayerId, decimal Stake);

internal sealed record FundsReserved(string BetId, decimal Amount);
