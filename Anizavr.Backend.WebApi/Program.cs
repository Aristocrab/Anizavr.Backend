using Calabonga.AspNetCore.AppDefinitions;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefinitions(typeof(Program));

var app = builder.Build();
app.UseDefinitions();

app.Run();