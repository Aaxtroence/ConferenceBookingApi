using ConferenceBookingApi.Services;
using ConferenceBookingApi.Exceptions;
using Xunit;

namespace ConferenceBookingApi.Tests;

public class PricingServiceTests
{
    private readonly PricingService _sut = new();

    [Fact]
    public void StandardHours_ChargesBaseRate()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0));

        Assert.Equal(4000, result);
    }

    [Fact]
    public void MorningDiscount_AppliesTenPercentOff()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 7, 0, 0),
            new DateTime(2026, 9, 1, 8, 0, 0));

        Assert.Equal(1800, result);
    }

    [Fact]
    public void PeakHours_ApplyFifteenPercentSurcharge()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 12, 0, 0),
            new DateTime(2026, 9, 1, 14, 0, 0));

        Assert.Equal(4600, result);
    }

    [Fact]
    public void EveningDiscount_AppliesTwentyPercentOff()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 19, 0, 0),
            new DateTime(2026, 9, 1, 20, 0, 0));

        Assert.Equal(1600, result);
    }

    [Fact]
    public void MixedRanges_CalculatesEachHourSeparately()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 8, 0, 0),
            new DateTime(2026, 9, 1, 15, 0, 0));

        Assert.Equal(14400, result);
    }

    [Fact]
    public void NonHourAlignedBooking_ChargesProportionally()
    {
        var result = _sut.CalculateRoomCost(2000,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 10, 30, 0));

        Assert.Equal(1000, result);
    }

    [Fact]
    public void BookingOutsideWorkingHours_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() =>
            _sut.CalculateRoomCost(2000,
                new DateTime(2026, 9, 1, 4, 0, 0),
                new DateTime(2026, 9, 1, 5, 0, 0)));
    }

    [Fact]
    public void EndTimeBeforeStartTime_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() =>
            _sut.CalculateRoomCost(2000,
                new DateTime(2026, 9, 1, 12, 0, 0),
                new DateTime(2026, 9, 1, 10, 0, 0)));
    }
}