namespace YachtLogHoursCalculator.Calculator;

public enum EntryType
{
    Moored,
    Sail,
    Engine
}

public static class EntryTypeExtensions
{
    public static string ToDisplayName(this EntryType entryType)
    {
        return entryType switch 
        {
            EntryType.Moored => "Postój",
            EntryType.Sail => "Żagle",
            EntryType.Engine => "Silnik",
            _ => throw new ArgumentOutOfRangeException(nameof(entryType), entryType, null)
        };
    }
}
