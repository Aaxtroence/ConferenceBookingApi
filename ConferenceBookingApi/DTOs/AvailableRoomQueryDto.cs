namespace ConferenceBookingApi.DTOs
{
    public class AvailableRoomQueryDto
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Capacity { get; set; }
    }
}
