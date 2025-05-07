using System.Text.Json;
using Blazored.LocalStorage;
using YachtLogHoursCalculator.Calculator;

namespace YachtLogHoursCalculator.Services;

public class StateService
{
    private readonly ILocalStorageService _localStorage;
    private const string StateKey = "yacht_log_state";
    
    public List<Day> Days { get; private set; } = new();
    public int CurrentDayNumber { get; set; } = 1;

    public Day CurrentDay => Days[CurrentDayNumber - 1];
    
    public bool IsLastDay => CurrentDayNumber == Days.Count;

    public StateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            var savedState = await _localStorage.GetItemAsync<SavedState>(StateKey);
            if (savedState != null)
            {
                Days = savedState.Days;
                CurrentDayNumber = Days.Count;
                
                // Ensure all days are properly recalculated after loading
                for (int i = 1; i < Days.Count; i++)
                {
                    CurrentDayNumber = i;
                    RecalculateFollowingDays();
                }
                
                // Reset to the last day
                CurrentDayNumber = Days.Count;
            }
            else
            {
                // Initialize with first day if no saved state exists
                InitializeEmptyState();
            }
        }
        catch
        {
            // If any error occurs during loading, start with fresh state
            InitializeEmptyState();
        }
    }

    private void InitializeEmptyState()
    {
        Days = [new Day()];
        CurrentDayNumber = 1;
    }

    public void OpenNewDay()
    {
        var previousDay = Days.Count > 0 
            ? Days.Last().GetNextDaySummary() 
            : LogEntrySet.Empty;
            
        var newDay = new Day(previousDay);
        
        // If we have previous days, initialize the new day with the last entry type
        if (Days.Count > 0 && Days.Last().Entries.Any())
        {
            var lastEntryType = Days.Last().Entries.Last().Type;
            newDay.Entries.Add(new LogEntry(TimeSpan.Zero, lastEntryType));
        }
        
        CurrentDayNumber = Days.Count + 1;
        Days.Add(newDay);
    }
    
    public async Task SaveStateAsync()
    {
        // Recalculate all days following the current day
        RecalculateFollowingDays();
        
        var state = new SavedState
        {
            Days = Days
        };
        
        await _localStorage.SetItemAsync(StateKey, state);
    }
    
    /// <summary>
    /// Recalculates all days that follow the current day
    /// to ensure they have the correct previous day data
    /// </summary>
    private void RecalculateFollowingDays()
    {
        // Nothing to recalculate if we're on the last day
        if (CurrentDayNumber >= Days.Count)
        {
            return;
        }
        
        // Start from the day after current day
        for (int i = CurrentDayNumber; i < Days.Count; i++)
        {
            var previousDay = i > 0 
                ? Days[i-1].GetNextDaySummary() 
                : LogEntrySet.Empty;
                
            // Update the previous day reference
            Days[i].PreviousDay = previousDay;
        }
    }
    
    public async Task ResetStateAsync()
    {
        await _localStorage.RemoveItemAsync(StateKey);
        InitializeEmptyState(); 
    }
    
    public void ViewNextDay()
    {
        if (CurrentDayNumber < Days.Count)
        {
            CurrentDayNumber++;
        }
    }
    
    public void ViewPreviousDay()
    {
        if (CurrentDayNumber > 1)
        {
            CurrentDayNumber--;
        }
    }
    
    private class SavedState
    {
        public List<Day> Days { get; set; } = new();
    }
} 