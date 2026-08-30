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

        [HttpPost]
        public async Task<ActionResult<BookingResultDto>> CreateBooking(CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(CreateBooking), new { id = result.Id }, result);
        }
    }
}