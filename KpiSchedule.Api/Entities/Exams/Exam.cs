namespace KpiSchedule.Api.Entities.Exams;

public class Exam
{
    public required Guid Id { get; set; }
    public required int DaysLeft { get; set; }
    public required DateTime Date { get; set; }
    public required string LecturerName { get; set; }
    public required Guid LecturerId { get; set; }
    public required string Room { get; set; }
    public required string SubjectShort { get; set; }
    public required string Subject { get; set; }
    public required object? Group { get; set; }
    public required string GenericGroupId { get; set; }
}