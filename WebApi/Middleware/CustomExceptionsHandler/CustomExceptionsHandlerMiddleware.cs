using System.Net;
using System.Text.Json;
using Application.Exceptions;
using FluentValidation;
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
            ArgumentException or InvalidOperationException or ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
        var errorMessage = exception.Message;
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        if (exception is InvalidOperationException invalidOperationException)
        {
            exception = invalidOperationException.InnerException ?? invalidOperationException;
        }
        if (exception is ValidationException validationException)
        {
            errorMessage = validationException.Errors.First().ErrorMessage;
        }

        var error = new
        {
            StatusCode = code,
            ErrorMessage = errorMessage
        };
        
        Log.Error("[{StatusCodeId} {StatusCode}] {ErrorMessage}", (int)error.StatusCode, error.StatusCode, error.ErrorMessage);

        var errorJson = JsonSerializer.Serialize(error);
        return context.Response.WriteAsync(errorJson);
    }
}