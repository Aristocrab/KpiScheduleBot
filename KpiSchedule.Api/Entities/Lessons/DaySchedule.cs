using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lessons;

public class DaySchedule
{
    [JsonPropertyName("Day")]
    public required string DayName { get; set; }
    
    [JsonPropertyName("Pairs")]
    public required List<Lesson> Lessons { get; set; }
}