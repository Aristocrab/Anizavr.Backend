namespace Anizavr.Backend.WebApi.Middleware.CustomExceptionsHandler;

public static class CustomExceptionsHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionsHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionsHandlerMiddleware>();
    }
}