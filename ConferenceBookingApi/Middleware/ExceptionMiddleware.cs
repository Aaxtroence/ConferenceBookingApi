using System.Net;
using System.Text.Json;
using ConferenceBookingApi.Exceptions;

namespace ConferenceBookingApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,       // 404
                ValidationException => HttpStatusCode.BadRequest,   // 400
                ConflictException => HttpStatusCode.Conflict,       // 409
                _ => HttpStatusCode.InternalServerError             // 500
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(result);
        }
    }
}