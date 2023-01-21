using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lessons;

public class Lessons
{
    [JsonPropertyName("GroupCode")] 
    public required Guid GroupId { get; set; }
    public required List<DayOfWeek> ScheduleFirstWeek { get; set; }
    public required List<DayOfWeek> ScheduleSecondWeek { get; set; }
}