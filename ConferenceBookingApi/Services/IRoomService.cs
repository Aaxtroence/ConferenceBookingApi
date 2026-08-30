using ConferenceBookingApi.DTOs;

namespace ConferenceBookingApi.Services
{
    public interface IRoomService
    {
        Task<RoomResultDto> CreateRoomAsync(CreateRoomDto dto);
        Task<RoomResultDto> UpdateRoomAsync(int id, UpdateRoomDto dto);
        Task DeleteRoomAsync(int id);
        Task<List<AvailableRoomResultDto>> GetAvailableRoomsAsync(AvailableRoomQueryDto query);

        Task<RevenueReportDto> GetRevenueReportAsync(int roomId, DateTime from, DateTime to);
        Task<OccupancyReportDto> GetOccupancyReportAsync(int roomId, DateTime from, DateTime to);
    }
}