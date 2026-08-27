namespace ConferenceBookingApi.DTOs;

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    public List<EquipmentDto> Equipment { get; set; } = new();
}