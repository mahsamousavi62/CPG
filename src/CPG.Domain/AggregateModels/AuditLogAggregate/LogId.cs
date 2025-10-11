using System;
using Ardalis.GuardClauses;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Value object representing a structured log identifier
/// Format: {Id}-{Domain}-{Abbreviation}
/// Example: 12345-Payment-IPG
/// </summary>
public record LogId
{
    public string Value { get; }

    private LogId()
    {
    }

    public LogId(string id, string domain, string abbreviation)
    {
        Guard.Against.NullOrWhiteSpace(id, nameof(id));
        Guard.Against.NullOrWhiteSpace(domain, nameof(domain));
        Guard.Against.NullOrWhiteSpace(abbreviation, nameof(abbreviation));

        Value = $"{id}-{domain}-{abbreviation}";
    }

    public LogId(string logId)
    {
        Guard.Against.NullOrWhiteSpace(logId, nameof(logId));

        var parts = logId.Split('-');
        if (parts.Length < 3)
            throw new ArgumentException("LogId must be in format: {Id}-{Domain}-{Abbreviation}", nameof(logId));

        Value = logId;
    }

    public static implicit operator string(LogId logId) => logId.Value;
    public static implicit operator LogId(string value) => new(value);

    public override string ToString() => Value;

    /// <summary>
    /// Parses the LogId and returns its components
    /// </summary>
    public (string Id, string Domain, string Abbreviation) Parse()
    {
        var parts = Value.Split('-');
        return (parts[0], parts[1], parts[2]);
    }
}
