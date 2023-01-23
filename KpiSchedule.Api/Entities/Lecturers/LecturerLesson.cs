namespace KpiSchedule.Api.Entities.Lecturers;

public class LecturerLesson
{
    public required string Group { get; set; }
    public required string Type { get; set; }
    public required string Time { get; set; }
    public required string Name { get; set; }
    public required string Place { get; set; }
    public required string Tag { get; set; }
}