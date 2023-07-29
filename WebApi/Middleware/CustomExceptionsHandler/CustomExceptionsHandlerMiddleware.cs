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
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        
        // Use LINQ inner exception
        if (exception is InvalidOperationException invalidOperationException)
        {
            exception = invalidOperationException.InnerException ?? invalidOperationException;
        }
        var errorMessage = exception.Message;
        // Show FluentValidation first error
        if (exception is ValidationException validationException)
        {
            errorMessage = validationException.Errors.First().ErrorMessage;
        }
        if (exception is ArgumentNullException)
        {
            errorMessage = "Ошибка валидации";
        }

        var error = new
        {
            StatusCode = code,
            ErrorMessage = errorMessage
        };
        
        Log.Error("{RequestMethod} {RequestPath} responded {StatusCode}. {ErrorMessage}",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, error.ErrorMessage);
        
        var errorJson = JsonSerializer.Serialize(error);
        return context.Response.WriteAsync(errorJson);
    }
}