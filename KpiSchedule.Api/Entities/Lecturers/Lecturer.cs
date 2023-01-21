using System.Text.Json.Serialization;

namespace KpiSchedule.Api.Entities.Lecturers;

public class Lecturer
{
    [JsonIgnore] public Guid? Id;

    [JsonPropertyName("Id")]
    public string? NullableId
    {
        set { Id = string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value); }
    }

    public required string Name { get; set; }
}