namespace ConferenceBookingApi.DTOs;

public class RevenueReportDto
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
}