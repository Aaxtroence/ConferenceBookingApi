using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Exceptions;
using ConferenceBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBookingApi.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly IPricingService _pricingService;

        public RoomService(AppDbContext context, IPricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
        }

        public async Task<RoomResultDto> CreateRoomAsync(CreateRoomDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Назва залу є обов'язковою");

            if (dto.Capacity <= 0)
                throw new ValidationException("Місткість залу має бути більше 0");

            var room = new Room
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                BasePricePerHour = dto.BasePricePerHour,
                Equipment = dto.Equipment?.Select(e => new Equipment
                {
                    Name = e.Name,
                    Price = e.Price
                }).ToList() ?? new List<Equipment>()
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task<RoomResultDto> UpdateRoomAsync(int id, UpdateRoomDto dto)
        {
            var room = await _context.Rooms
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
                throw new NotFoundException($"Зал id={id} не знайдено");

            if (dto.Name is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ValidationException("Назва залу не може бути порожньою");
                room.Name = dto.Name;
            }

            if (dto.Capacity is not null)
            {
                if (dto.Capacity <= 0)
                    throw new ValidationException("Місткість залу має бути більше 0");
                room.Capacity = dto.Capacity.Value;
            }

            if (dto.BasePricePerHour is not null)
                room.BasePricePerHour = dto.BasePricePerHour.Value;

            await _context.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
                throw new NotFoundException($"Зал id={id} не знайдено");

            var hasActiveBookings = room.Bookings.Any(b => b.EndTime > DateTime.UtcNow);
            if (hasActiveBookings)
                throw new ConflictException("Видалення неможливе. У залі є активні бронювання");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        private static RoomResultDto MapToDto(Room room)
        {
            return new RoomResultDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                BasePricePerHour = room.BasePricePerHour,
                Equipment = room.Equipment?.Select(e => new EquipmentDto
                {
                    Name = e.Name,
                    Price = e.Price
                }).ToList() ?? new List<EquipmentDto>()
            };
        }
        public async Task<List<AvailableRoomResultDto>> GetAvailableRoomsAsync(AvailableRoomQueryDto query)
        {
            if (query.EndTime <= query.StartTime)
                throw new ValidationException("Кінцевий час має бути більшим за початковий");

            var requestedStart = query.Date.Date + query.StartTime;
            var requestedEnd = query.Date.Date + query.EndTime;

            var rooms = await _context.Rooms
                .Include(r => r.Bookings)
                .Where(r => r.Capacity >= query.Capacity)
                .ToListAsync();

            var available = rooms.Where(r => !r.Bookings.Any(b =>
                Overlaps(b, requestedStart, requestedEnd)));

            return available.Select(r => new AvailableRoomResultDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                BasePricePerHour = r.BasePricePerHour
            }).ToList();
        }

        private static bool Overlaps(Booking booking, DateTime start, DateTime end)
        {
            return booking.StartTime < end && start < booking.EndTime;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(int roomId, DateTime from, DateTime to)
        {
            if (to <= from)
                throw new ValidationException("Кінцева дата має бути пізніше за початкову");

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null)
                throw new NotFoundException($"Зал id={roomId} не знайдено");

            var bookingsInRange = room.Bookings
                .Where(b => b.StartTime >= from && b.StartTime < to)
                .ToList();

            return new RevenueReportDto
            {
                RoomId = room.Id,
                RoomName = room.Name,
                From = from,
                To = to,
                TotalBookings = bookingsInRange.Count,
                TotalRevenue = bookingsInRange.Sum(b => b.TotalPrice)
            };
        }

        public async Task<OccupancyReportDto> GetOccupancyReportAsync(int roomId, DateTime from, DateTime to)
        {
            if (to <= from)
                throw new ValidationException("Кінцева дата має бути пізніше за початкову");

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null)
                throw new NotFoundException($"Зал id={roomId} не знайдено");

            var bookingsInRange = room.Bookings
                .Where(b => b.StartTime >= from && b.StartTime < to)
                .ToList();

            var bookedHours = bookingsInRange.Sum(b => (b.EndTime - b.StartTime).TotalHours);

            var workingHoursPerDay = _pricingService.GetWorkingHoursPerDay();
            var totalDays = Math.Max(1, Math.Ceiling((to - from).TotalDays));
            var availableHours = totalDays * workingHoursPerDay;

            var occupancyPercentage = availableHours > 0
                ? Math.Round(bookedHours / availableHours * 100, 2)
                : 0;

            return new OccupancyReportDto
            {
                RoomId = room.Id,
                RoomName = room.Name,
                From = from,
                To = to,
                BookedHours = bookedHours,
                AvailableHours = availableHours,
                OccupancyPercentage = occupancyPercentage
            };
        }
    }
}