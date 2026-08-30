namespace ConferenceBookingApi.DTOs
{
    public class AvailableRoomResultDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal BasePricePerHour { get; set; }
    }
}
