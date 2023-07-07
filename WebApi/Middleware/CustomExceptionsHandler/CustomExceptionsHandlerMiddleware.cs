using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Serilog;

namespace WebApi.Middleware.CustomExceptionsHandler;

public class CustomExceptionsHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionsHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExcepitonAsync(context, exception);
        }
    }

    private static Task HandleExcepitonAsync(HttpContext context, Exception exception)
    {
        var code = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            UserAlreadyExistsException => HttpStatusCode.Conflict,
            WrongPasswordException => HttpStatusCode.Unauthorized,
            ArgumentException or InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        if (exception is InvalidOperationException invalidOperationException)
        {
            exception = invalidOperationException.InnerException ?? invalidOperationException;
        }

        var error = new
        {
            StatusCode = code,
            ErrorMessage = exception.Message
        };
        
        Log.Error("[{StatusCodeId} {StatusCode}] {ErrorMessage}", (int)error.StatusCode, error.StatusCode, error.ErrorMessage);

        var errorJson = JsonSerializer.Serialize(error);
        return context.Response.WriteAsync(errorJson);
    }
}