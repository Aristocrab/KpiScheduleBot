namespace KpiSchedule.Database;

public class ChatSettings
{
    public long ChatId { get; set; }
    public Guid GroupId { get; set; }
    public required string GroupCode { get; set; }
}