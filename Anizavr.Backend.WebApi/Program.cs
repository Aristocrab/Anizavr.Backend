using Microsoft.Extensions.FileProviders;
using Anizavr.Backend.WebApi;
using Anizavr.Backend.WebApi.Middleware.CustomExceptionsHandler;
using Anizavr.Backend.WebApi.Middleware.RequestLogging;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

var app = builder.Build();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// CORS
app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyHeader();
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyMethod();
});

// Custom middleware
app.UseCustomExceptionsHandler();
app.UseRequestLogging();

// Caching
app.UseResponseCaching();

// Avatars
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/Avatars")),
//     RequestPath = new PathString("/avatars")
// });

// Api
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.MapControllers();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();