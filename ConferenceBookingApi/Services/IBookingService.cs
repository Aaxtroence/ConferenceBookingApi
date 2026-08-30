using ConferenceBookingApi.DTOs;

namespace ConferenceBookingApi.Services;

public interface IBookingService
{
    Task<BookingResultDto> CreateBookingAsync(CreateBookingDto dto);
}