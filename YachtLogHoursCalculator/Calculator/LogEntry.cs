using Microsoft.Extensions.Logging.Abstractions;

namespace YachtLogHoursCalculator.Calculator;

/// <summary>
/// Represents log event in time, i.e. putting sail up, putting sail down, engine on, engine off.
/// </summary>
/// <param name="StartTime">Time of the day when event happened</param>
/// <param name="Type">Type of events</param>
public record LogEntry(TimeSpan StartTime, EntryType Type)
{
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
    

    private LogEntrySet(Dictionary<EntryType, TimeSpan> entryDurations)
    {
        Moored = entryDurations.GetValueOrDefault(EntryType.Moored);
        Engine = entryDurations.GetValueOrDefault(EntryType.Engine);
        Sails = entryDurations.GetValueOrDefault(EntryType.Sail);
    }
    
    public TimeSpan Sails { get; }
    public TimeSpan Moored { get; }
    public TimeSpan Engine { get; }
    
    public TimeSpan SailsAndEngine => Sails + Engine;

    public TimeSpan Sum => Sails + Engine + Moored;
    public static LogEntrySet Empty { get; } = new(new Dictionary<EntryType, TimeSpan>());
}