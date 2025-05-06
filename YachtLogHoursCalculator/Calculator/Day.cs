using System;
using System.Collections.Generic;

namespace YachtLogHoursCalculator.Calculator;

/// <summary>
/// Represents a single day with its log entries
/// </summary>
public class Day
{
    public DateTime Date { get; }
    public List<LogEntry> Entries { get; } = new();
    public LogEntrySet PreviousDay { get; set; } = LogEntrySet.Empty;

    public Day(DateTime date, LogEntrySet previousDay)
    {
        Date = date;
        PreviousDay = previousDay;
    }

    public LogEntrySet GetSummary()
    {
        return LogEntrySet.FromEntries(Entries);
    }

    public LogEntrySet GetNextDaySummary()
    {
        return LogEntrySet.FromEntries(Entries, PreviousDay);
    }

    public string FormattedDate => Date.ToString("yyyy-MM-dd");
} 