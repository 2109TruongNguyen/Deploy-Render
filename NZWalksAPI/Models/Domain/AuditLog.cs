namespace NZWalksAPI.Models.Domain;

public class AuditLog
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public required string EntityName { get; set; }
    public required string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public required string Changes { get; set; }
}