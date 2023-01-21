﻿using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lecturers;

public class LecturerInfo
{
    [JsonPropertyName("LecturerName")] 
    public Guid LecturerId { get; set; }
    
    public required List<DayOfWeek> ScheduleFirstWeek { get; set; }
    public required List<DayOfWeek> ScheduleSecondWeek { get; set; }
}