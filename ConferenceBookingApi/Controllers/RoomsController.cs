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

        /// <summary>Створити новий конференц-зал</summary>
        /// <param name="dto">Назва, місткість, базова вартість за годину та список обладнання</param>
        [HttpPost]
        [ProducesResponseType(typeof(RoomResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoomResultDto>> CreateRoom(CreateRoomDto dto)
        {
            var result = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(CreateRoom), new { id = result.Id }, result);
        }

        /// <summary>Знайти вільні зали за датою, часом і мінімальною місткістю</summary>
        /// <param name="query">Дата, час початку/завершення та потрібна місткість</param>
        [HttpGet("available")]
        [ProducesResponseType(typeof(List<AvailableRoomResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AvailableRoomResultDto>>> GetAvailableRooms(
            [FromQuery] AvailableRoomQueryDto query)
        {
            var result = await _roomService.GetAvailableRoomsAsync(query);
            return Ok(result);
        }

        /// <summary>Оновити дані залу (тільки передані поля)</summary>
        /// <param name="id">Id залу</param>
        /// <param name="dto">Поля для оновлення — всі опційні</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoomResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomResultDto>> UpdateRoom(int id, UpdateRoomDto dto)
        {
            var result = await _roomService.UpdateRoomAsync(id, dto);
            return Ok(result);
        }

        /// <summary>Видалити зал. Заборонено, якщо є активні бронювання</summary>
        /// <param name="id">Id залу</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return NoContent();
        }
    }
}