using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lessons;

public class Lesson
{
    public required string TeacherName { get; set; }
    [JsonIgnore] public Guid? LecturerId;
    [JsonPropertyName("LecturerId")]
    public string? NullableLecturerId
    {
        set => LecturerId = string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value);
    }
    public required string Type { get; set; }
    public required string Time { get; set; }
    public required string Name { get; set; }
    public required string Place { get; set; }
    public required string Tag { get; set; }
}