namespace ConferenceBookingApi.DTOs;

public class RoomResultDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    public List<EquipmentDto> Equipment { get; set; } = new();
}