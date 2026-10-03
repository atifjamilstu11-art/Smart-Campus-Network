namespace SmartCampusNetwork.API.Models;

public class FaultAnalysisResult
{
    public int FailedDeviceId { get; set; }
    public string FailedDeviceName { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<int> ReachableDeviceIds { get; set; } = new();
    public List<int> UnreachableDeviceIds { get; set; } = new();
    public List<string> AlternativeRoutes { get; set; } = new();
    public string Severity { get; set; } = "Medium";
    public int Priority { get; set; }
}
