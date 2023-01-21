namespace KpiSchedule.Api.Entities.Exams;

public class ExamsDto
{
    public required Paging Paging { get; set; }
    public required List<Exam> Data { get; set; }
}