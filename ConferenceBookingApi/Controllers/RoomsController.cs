using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<ActionResult<RoomResultDto>> CreateRoom(CreateRoomDto dto)
        {
            var result = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(CreateRoom), new { id = result.Id }, result);
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<AvailableRoomResultDto>>> GetAvailableRooms(
            [FromQuery] AvailableRoomQueryDto query)
        {
            var result = await _roomService.GetAvailableRoomsAsync(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RoomResultDto>> UpdateRoom(int id, UpdateRoomDto dto)
        {
            var result = await _roomService.UpdateRoomAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return NoContent();
        }
    }
}