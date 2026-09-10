using System.Reflection;

namespace EventContracts.Tests;

public sealed class PublicApiArchitectureTests
{
    [Fact]
    public void Assembly_exposes_only_the_nine_accepted_contract_types()
    {
        var exportedTypeNames = typeof(EventId).Assembly.ExportedTypes
            .Select(type => type.IsGenericType ? type.Name[..type.Name.IndexOf('`')] : type.Name)
            .OrderBy(name => name)
            .ToArray();

        string[] expected =
        [
            nameof(CausationId),
            nameof(CorrelationId),
            nameof(EventEnvelope<object>),
            nameof(EventId),
            nameof(EventMetadata),
            nameof(EventTypeName),
            nameof(SchemaVersion),
            nameof(TenantId),
            nameof(TraceContext),
        ];

        Assert.Equal(expected.OrderBy(name => name), exportedTypeNames);
    }

    [Fact]
    public void Assembly_has_no_public_interfaces_or_domain_payloads()
    {
        var exportedTypes = typeof(EventId).Assembly.ExportedTypes.ToArray();

        Assert.DoesNotContain(exportedTypes, type => type.IsInterface);
        Assert.DoesNotContain(exportedTypes, type => type.Name == "IEvent");
        Assert.DoesNotContain(exportedTypes, type => type.Name is "BetPlaced" or "FundsReserved" or "PlayerRegistered");
    }

    [Fact]
    public void Envelope_has_only_metadata_and_payload_properties()
    {
        var propertyNames = typeof(EventEnvelope<>).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name)
            .OrderBy(name => name);

        Assert.Equal(new[] { "Metadata", "Payload" }, propertyNames);
    }

    [Fact]
    public void Core_has_no_forbidden_framework_dependencies()
    {
        var references = typeof(EventId).Assembly.GetReferencedAssemblies().Select(reference => reference.Name).ToArray();
        string[] forbiddenFragments =
        [
            "Kafka",
            "Rabbit",
            "MassTransit",
            "MediatR",
            "DependencyInjection",
            "EntityFramework",
            "OpenTelemetry",
            "System.Text.Json",
        ];

        Assert.DoesNotContain(references, reference =>
            forbiddenFragments.Any(fragment => reference?.Contains(fragment, StringComparison.OrdinalIgnoreCase) == true));
    }
}
