namespace ConferenceBookingApi.DTOs;

public class UpdateRoomDto
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public decimal? BasePricePerHour { get; set; }
}