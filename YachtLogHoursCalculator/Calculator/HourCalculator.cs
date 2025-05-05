namespace YachtLogHoursCalculator.Calculator;

public static class HourCalculator
{
    public static Dictionary<EntryType, TimeSpan> Calculate(IEnumerable<LogEntry> entries)
    {
        var sorted = entries.OrderBy(n => n.StartTime).ToList();

        // Entry duration is calculated as the difference between the entry and the next entry
        // This last entry handles last entry in the day nicely
        var last = new LogEntry(TimeSpan.FromHours(24), (EntryType.Moored));
        sorted.Add(last);

        if (!sorted.Any())
        {
            return new Dictionary<EntryType, TimeSpan>();
        }

        Dictionary<EntryType, TimeSpan> hourSum = new();

        var currentEntry = sorted.First();

        foreach (var nextEntry in sorted.Skip(1))
        {
            var elapsed = nextEntry.StartTime - currentEntry.StartTime;
            hourSum[currentEntry.Type] = hourSum.GetValueOrDefault(currentEntry.Type) + elapsed;

            currentEntry = nextEntry;
        }
            
        return hourSum;
    }
}