using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lessons;

public class Lessons
{
    [JsonPropertyName("GroupCode")] 
    public required Guid GroupId { get; set; }
    public required List<DaySchedule> ScheduleFirstWeek { get; set; }
    public required List<DaySchedule> ScheduleSecondWeek { get; set; }
}