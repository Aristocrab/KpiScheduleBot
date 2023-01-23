using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lecturers;

public class LecturerInfo
{
    [JsonPropertyName("LecturerName")] 
    public Guid LecturerId { get; set; }
    
    public required List<LecturerDaySchedule> ScheduleFirstWeek { get; set; }
    public required List<LecturerDaySchedule> ScheduleSecondWeek { get; set; }
}