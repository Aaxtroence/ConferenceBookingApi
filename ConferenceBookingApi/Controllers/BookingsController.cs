using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBookingApi.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Забронювати зал. Перевіряє конфлікт часу, розраховує вартість
        /// (оренда залу по годинах + обране обладнання) і зберігає бронювання.
        /// </summary>
        /// <param name="dto">Id залу, час початку/завершення, обране обладнання</param>
        [HttpPost]
        [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookingResultDto>> CreateBooking(CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(CreateBooking), new { id = result.Id }, result);
        }
    }
}