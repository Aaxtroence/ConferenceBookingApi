using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBookingApi.Tests;

public class BookingServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateBooking_RoomNotFound_ThrowsNotFoundException()
    {
        await using var context = CreateContext();
        var service = new BookingService(context, new PricingService());

        var dto = new CreateBookingDto
        {
            RoomId = 999,
            StartTime = new DateTime(2026, 9, 1, 10, 0, 0),
            EndTime = new DateTime(2026, 9, 1, 12, 0, 0)
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateBookingAsync(dto));
    }

    [Fact]
    public async Task CreateBooking_OverlappingTime_ThrowsConflictException()
    {
        await using var context = CreateContext();

        var room = new Room { Name = "Зал А", Capacity = 50, BasePricePerHour = 2000 };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        context.Bookings.Add(new Booking
        {
            RoomId = room.Id,
            StartTime = new DateTime(2026, 9, 1, 10, 0, 0),
            EndTime = new DateTime(2026, 9, 1, 12, 0, 0),
            TotalPrice = 4000
        });
        await context.SaveChangesAsync();

        var service = new BookingService(context, new PricingService());

        var dto = new CreateBookingDto
        {
            RoomId = room.Id,
            StartTime = new DateTime(2026, 9, 1, 11, 0, 0),
            EndTime = new DateTime(2026, 9, 1, 13, 0, 0)
        };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateBookingAsync(dto));
    }

    [Fact]
    public async Task CreateBooking_ValidRequest_CalculatesTotalPriceCorrectly()
    {
        await using var context = CreateContext();

        var room = new Room { Name = "Зал А", Capacity = 50, BasePricePerHour = 2000 };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var service = new BookingService(context, new PricingService());

        var dto = new CreateBookingDto
        {
            RoomId = room.Id,
            StartTime = new DateTime(2026, 9, 1, 10, 0, 0),
            EndTime = new DateTime(2026, 9, 1, 12, 0, 0)
        };

        var result = await service.CreateBookingAsync(dto);

        Assert.Equal(4000, result.TotalPrice);
    }
}