namespace ConferenceBookingApi.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
