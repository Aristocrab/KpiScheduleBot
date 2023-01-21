namespace KpiSchedule.Api.Entities.Groups;

public class Group
{
    public required Guid Id { get; set; } 
    public required string Name { get; set; }
    public required string Faculty { get; set; }
}