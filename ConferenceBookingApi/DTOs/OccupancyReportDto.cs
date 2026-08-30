namespace ConferenceBookingApi.DTOs;

public class OccupancyReportDto
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public double BookedHours { get; set; }
    public double AvailableHours { get; set; }
    public double OccupancyPercentage { get; set; }
}