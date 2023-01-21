namespace KpiSchedule.Api.Entities.Lecturers;

public class LecturersDto
{
    public required Paging Paging { get; set; }
    public required List<Lecturer> Data { get; set; }
}