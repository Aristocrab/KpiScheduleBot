namespace KpiSchedule.Api.Entities.Lessons;

public class DayOfWeek
{
    public required string Day { get; set; }
    public required List<Lesson> Pairs { get; set; }
}