namespace KpiSchedule.Api.Entities.Lessons;

public class Lesson
{
    public required string TeacherName { get; set; }
    public required Guid LecturerId { get; set; }
    public required string Type { get; set; }
    public required string Time { get; set; }
    public required string Name { get; set; }
    public required string Place { get; set; }
    public required string Tag { get; set; }
}