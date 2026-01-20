using Domain.Common;

namespace Domain.Entities;

public class GameWeek : Entity
{
    public int WeekNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCompleted { get; private set; }

    private GameWeek() { } // For EF Core

    public GameWeek(int weekNumber, DateTime startDate, DateTime endDate)
    {
        if (weekNumber <= 0)
            throw new ArgumentException("Week number must be greater than zero", nameof(weekNumber));
        
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");

        WeekNumber = weekNumber;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = false;
        IsCompleted = false;
    }

    public void Activate()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot activate a completed game week");
        
        IsActive = true;
    }

    public void Complete()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot complete an inactive game week");
        
        IsActive = false;
        IsCompleted = true;
    }
}
