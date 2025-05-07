using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json.Serialization;

namespace YachtLogHoursCalculator.Calculator;

/// <summary>
/// Represents log event in time, i.e. putting sail up, putting sail down, engine on, engine off.
/// </summary>
/// <param name="StartTime">Time of the day when event happened</param>
/// <param name="Type">Type of events</param>
public record LogEntry(TimeSpan StartTime, EntryType Type)
{
    [JsonConstructor]
    public LogEntry() : this(TimeSpan.Zero, EntryType.Moored)
    {
    }
}


/// <summary>
/// Represents set of entries to be put in yach log, can represent past aggregation, current day or value to transfer to next day. 
/// </summary>
public record LogEntrySet
{
    public static LogEntrySet FromEntries(ICollection<LogEntry> entries, LogEntrySet? previousDay = null)
    {
        var durations = HourCalculator.Calculate(entries);
        
        // Add previous day values to current day
        durations[EntryType.Moored] = durations.GetValueOrDefault(EntryType.Moored) + (previousDay?.Moored ?? TimeSpan.Zero);
        durations[EntryType.Engine] = durations.GetValueOrDefault(EntryType.Engine) + (previousDay?.Engine ?? TimeSpan.Zero);
        durations[EntryType.Sail] = durations.GetValueOrDefault(EntryType.Sail) + (previousDay?.Sails ?? TimeSpan.Zero);

        return new LogEntrySet(durations);
    }
    
    [JsonConstructor]
    public LogEntrySet()
    {
        Sails = TimeSpan.Zero;
        Moored = TimeSpan.Zero;
        Engine = TimeSpan.Zero;
    }

    private LogEntrySet(Dictionary<EntryType, TimeSpan> entryDurations)
    {
        Moored = entryDurations.GetValueOrDefault(EntryType.Moored);
        Engine = entryDurations.GetValueOrDefault(EntryType.Engine);
        Sails = entryDurations.GetValueOrDefault(EntryType.Sail);
    }
    
    [JsonInclude]
    public TimeSpan Sails { get; set; }
    
    [JsonInclude]
    public TimeSpan Moored { get; set; }
    
    [JsonInclude]
    public TimeSpan Engine { get; set; }
    
    public TimeSpan SailsAndEngine => Sails + Engine;

    public TimeSpan Sum => Sails + Engine + Moored;
    
    public static LogEntrySet Empty { get; } = new();
}