using ConferenceBookingApi.DTOs;

namespace ConferenceBookingApi.Services
{
    public interface IRoomService
    {
        Task<RoomResultDto> CreateRoomAsync(CreateRoomDto dto);
        Task<RoomResultDto> UpdateRoomAsync(int id, UpdateRoomDto dto);
        Task DeleteRoomAsync(int id);
    }
}