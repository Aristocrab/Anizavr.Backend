using System.Net;
using System.Text.Json;
using AspNetCore.AppModules;
using Anizavr.Backend.Application.Exceptions;
using FluentValidation;
using Serilog;

namespace Anizavr.Backend.WebApi.Modules.ExceptionsHandling;

public class ExceptionsHandlingModule : AppModule
{
    public override int OrderIndex => -1;

    public override void ConfigureApplication(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch(Exception exception)
            {
                await HandleException(context, exception);
            }
        });
    }
    
    private static async Task HandleException(HttpContext context, Exception exception)
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

        var errorMessage = exception switch
        {
            ValidationException validationException => validationException.Errors.First().ErrorMessage,
            ArgumentNullException => "Ошибка валидации",
            _ => exception.Message
        };

        var error = new
        {
            StatusCode = code,
            ErrorMessage = errorMessage
        };
        
        Log.Error("{RequestMethod} {RequestPath} responded {StatusCode}. {ErrorMessage}",
            context.Request.Method, 
            context.Request.Path, 
            context.Response.StatusCode, 
            error.ErrorMessage);
        
        var errorJson = JsonSerializer.Serialize(error);
        await context.Response.WriteAsync(errorJson);
    }
}