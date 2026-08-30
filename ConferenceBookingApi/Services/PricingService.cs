using ConferenceBookingApi.Exceptions;

namespace ConferenceBookingApi.Services;

public class PricingService : IPricingService
{
    private const int WorkingStartHour = 6;
    private const int WorkingEndHour = 23;

    public decimal CalculateRoomCost(decimal basePricePerHour, DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ValidationException("Кінцевий час має бути пізніше за початковий");

        decimal total = 0;
        var current = start;

        while (current < end)
        {
            var nextHourBoundary = current.Date.AddHours(current.Hour + 1);
            var segmentEnd = nextHourBoundary < end ? nextHourBoundary : end;

            var fractionOfHour = (decimal)(segmentEnd - current).TotalHours;
            var multiplier = GetMultiplier(current.Hour);

            total += basePricePerHour * multiplier * fractionOfHour;

            current = segmentEnd;
        }

        return total;
    }

    public double GetWorkingHoursPerDay()
    {
        return WorkingEndHour - WorkingStartHour;
    }

    private static decimal GetMultiplier(int hour)
    {
        if (hour < WorkingStartHour || hour >= WorkingEndHour)
            throw new ValidationException($"Допустимий час для бронювання: з {WorkingStartHour}:00 до {WorkingEndHour}:00");

        return hour switch
        {
            >= 6 and < 9 => 0.90m,
            >= 9 and < 12 => 1.00m,
            >= 12 and < 14 => 1.15m,
            >= 14 and < 18 => 1.00m,
            >= 18 and < 23 => 0.80m,
            _ => 1.00m
        };
    }
}