namespace YachtLogHoursCalculator.Calculator
{
    public record LogEntry(TimeSpan StartTime, string Type)
    {
        public static List<string> Types = new List<string>() { "Postój", "Żagle", "Silnik", "Żagle + Silnik" };
    }
}
