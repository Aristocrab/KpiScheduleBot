namespace KpiSchedule.Api.Entities.Groups;

public class GroupsDto
{
    public required Paging Paging { get; set; }
    public required List<Group> Data { get; set; }
}