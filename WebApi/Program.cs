using Serilog;
using WebApi;
using WebApi.Middleware.CustomExceptionsHandler;

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
app.UseSerilogRequestLogging();
app.UseResponseCaching();

app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();