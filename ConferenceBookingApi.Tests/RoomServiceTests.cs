using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBookingApi.Tests;

public class RoomServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateRoom_ValidData_CreatesRoomSuccessfully()
    {
        await using var context = CreateContext();
        var service = new RoomService(context);

        var dto = new CreateRoomDto
        {
            Name = "Зал А",
            Capacity = 50,
            BasePricePerHour = 2000
        };

        var result = await service.CreateRoomAsync(dto);

        Assert.Equal("Зал А", result.Name);
        Assert.Equal(50, result.Capacity);
        Assert.Equal(2000, result.BasePricePerHour);
    }

    [Fact]
    public async Task CreateRoom_EmptyName_ThrowsValidationException()
    {
        await using var context = CreateContext();
        var service = new RoomService(context);

        var dto = new CreateRoomDto { Name = "", Capacity = 50, BasePricePerHour = 2000 };

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateRoomAsync(dto));
    }

    [Fact]
    public async Task UpdateRoom_RoomNotFound_ThrowsNotFoundException()
    {
        await using var context = CreateContext();
        var service = new RoomService(context);

        var dto = new UpdateRoomDto { Name = "Нова назва" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateRoomAsync(999, dto));
    }

    [Fact]
    public async Task DeleteRoom_HasActiveBooking_ThrowsConflictException()
    {
        await using var context = CreateContext();

        var room = new Room { Name = "Зал А", Capacity = 50, BasePricePerHour = 2000 };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        context.Bookings.Add(new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            TotalPrice = 4000
        });
        await context.SaveChangesAsync();

        var service = new RoomService(context);

        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteRoomAsync(room.Id));
    }

    [Fact]
    public async Task GetAvailableRooms_ExcludesRoomsBelowRequestedCapacity()
    {
        await using var context = CreateContext();

        context.Rooms.AddRange(
            new Room { Name = "Малий", Capacity = 10, BasePricePerHour = 1000 },
            new Room { Name = "Великий", Capacity = 100, BasePricePerHour = 3000 }
        );
        await context.SaveChangesAsync();

        var service = new RoomService(context);

        var query = new AvailableRoomQueryDto
        {
            Date = new DateTime(2026, 9, 1),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Capacity = 50
        };

        var result = await service.GetAvailableRoomsAsync(query);

        Assert.Single(result);
        Assert.Equal("Великий", result[0].Name);
    }
}