using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly IPricingService _pricingService;

    public BookingService(AppDbContext context, IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var room = await _context.Rooms
            .Include(r => r.Equipment)
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

        if (room is null)
            throw new NotFoundException($"Зал id={dto.RoomId} не знайдено");

        if (dto.EndTime <= dto.StartTime)
            throw new ValidationException("Кінцевий час має бути пізніше за початковий");

        var hasConflict = room.Bookings.Any(b =>
            b.StartTime < dto.EndTime && dto.StartTime < b.EndTime);

        if (hasConflict)
            throw new ConflictException("Обраний час вже зайнятий для цього залу");

        var selectedEquipment = room.Equipment
            .Where(e => dto.SelectedEquipmentIds.Contains(e.Id))
            .ToList();

        if (selectedEquipment.Count != dto.SelectedEquipmentIds.Count)
            throw new ValidationException("Деяке з обраного обладнання не належить цьому залу або не існує");

        var roomCost = _pricingService.CalculateRoomCost(room.BasePricePerHour, dto.StartTime, dto.EndTime);
        var equipmentCost = selectedEquipment.Sum(e => e.Price);
        var totalPrice = roomCost + equipmentCost;

        var booking = new Booking
        {
            RoomId = room.Id,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            SelectedEquipment = selectedEquipment,
            TotalPrice = totalPrice
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return new BookingResultDto
        {
            Id = booking.Id,
            RoomId = room.Id,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            RoomCost = roomCost,
            EquipmentCost = equipmentCost,
            TotalPrice = totalPrice,
            SelectedEquipmentNames = selectedEquipment.Select(e => e.Name).ToList()
        };
    }
}