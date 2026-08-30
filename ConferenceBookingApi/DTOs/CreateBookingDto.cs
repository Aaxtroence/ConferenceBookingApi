namespace ConferenceBookingApi.DTOs;

public class CreateBookingDto
{
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<int> SelectedEquipmentIds { get; set; } = new();
}