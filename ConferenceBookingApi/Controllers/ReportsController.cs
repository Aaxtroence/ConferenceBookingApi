using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public ReportsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>Сумарний дохід по залу за період</summary>
        /// <param name="roomId">Id залу</param>
        /// <param name="from">Початок періоду</param>
        /// <param name="to">Кінець періоду</param>
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RevenueReportDto>> GetRevenue(
            int roomId, DateTime from, DateTime to)
        {
            var result = await _roomService.GetRevenueReportAsync(roomId, from, to);
            return Ok(result);
        }

        /// <summary>Відсоток зайнятості залу за період</summary>
        /// <param name="roomId">Id залу</param>
        /// <param name="from">Початок періоду</param>
        /// <param name="to">Кінець періоду</param>
        [HttpGet("occupancy")]
        [ProducesResponseType(typeof(OccupancyReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OccupancyReportDto>> GetOccupancy(
            int roomId, DateTime from, DateTime to)
        {
            var result = await _roomService.GetOccupancyReportAsync(roomId, from, to);
            return Ok(result);
        }
    }
}