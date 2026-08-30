namespace ConferenceBookingApi.DTOs;

public class BookingResultDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal RoomCost { get; set; }
    public decimal EquipmentCost { get; set; }
    public decimal TotalPrice { get; set; }
    public List<string> SelectedEquipmentNames { get; set; } = new();
}