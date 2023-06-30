using System.Net;
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
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;

        Log.Error(exception.Message);

        return context.Response.WriteAsync(exception.Message);
    }
}