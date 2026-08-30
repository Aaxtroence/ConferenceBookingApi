namespace ConferenceBookingApi.Services
{
    public interface IPricingService
    {
        decimal CalculateRoomCost(decimal basePricePerHour, DateTime start, DateTime end);
        double GetWorkingHoursPerDay();
    }
}
