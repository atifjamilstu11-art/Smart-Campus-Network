namespace SmartCampusNetwork.API.Models;

public class Connection
{
    public int Id { get; set; }
    public int SourceDeviceId { get; set; }
    public int TargetDeviceId { get; set; }
    public int Cost { get; set; } = 1;
    public string Status { get; set; } = "Active";
}
