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

        public RoomService(AppDbContext context)
        {
            _context = context;
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
    }
}