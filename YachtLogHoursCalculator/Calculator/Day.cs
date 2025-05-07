using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YachtLogHoursCalculator.Calculator;

/// <summary>
/// Represents a single day with its log entries
/// </summary>
public class Day
{
    [JsonInclude]
    public List<LogEntry> Entries { get; set; } = new();
    
    [JsonInclude]
    public LogEntrySet PreviousDay { get; set; }

    [JsonConstructor]
    public Day(LogEntrySet? previousDay = null)
    {
        PreviousDay = previousDay ?? LogEntrySet.Empty;
    }
    
    public LogEntrySet GetSummary()
    {
        return LogEntrySet.FromEntries(Entries);
    }

    public LogEntrySet GetNextDaySummary()
    {
        return LogEntrySet.FromEntries(Entries, PreviousDay);
    }
} 