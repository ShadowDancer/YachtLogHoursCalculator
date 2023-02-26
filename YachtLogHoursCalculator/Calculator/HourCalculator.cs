namespace YachtLogHoursCalculator.Calculator
{
    public static class HourCalculator
    {
        public static Dictionary<string, TimeSpan> Calculate(IEnumerable<LogEntry> entries)
        {
            var sorted = entries.OrderBy(n => n.StartTime).ToList();

            var last = new LogEntry(TimeSpan.FromHours(24), "end of day senteniel");
            sorted.Add(last);

            if (!sorted.Any())
            {
                return new Dictionary<string, TimeSpan>();
            }

            Dictionary<string, TimeSpan> hourSum = new();

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
}
