using System.Text.Json.Serialization;
using KpiSchedule.Api.Entities.Lessons;

namespace KpiSchedule.Api.Entities.Lecturers;

public class LecturerDaySchedule
{
    [JsonPropertyName("Day")]
    public required string DayName { get; set; }
    
    [JsonPropertyName("Pairs")]
    public required List<LecturerLesson> Lessons { get; set; }
}