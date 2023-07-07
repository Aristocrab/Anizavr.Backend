using Microsoft.Extensions.FileProviders;
using WebApi;
using WebApi.Middleware.CustomExceptionsHandler;
using WebApi.Middleware.RequestLogging;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionsHandler();
app.UseCors(b =>
{
    b.AllowAnyHeader();
    b.AllowAnyOrigin();
    b.AllowAnyMethod();
});
app.UseRequestLogging();
app.UseResponseCaching();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/Avatars")),
    RequestPath = new PathString("/avatars")
});

app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();