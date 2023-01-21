namespace KpiSchedule.Api.Entities.Time;

public class CurrentTime
{
    public required int CurrentWeek { get; set; }
    public required int CurrentDay { get; set; }
    public required int CurrentLesson { get; set; }
}