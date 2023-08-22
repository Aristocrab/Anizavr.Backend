using System.Globalization;
using Anizavr.Backend.Application.Validators;
using Aristocrab.AspNetCore.AppModules;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Modules.FluentValidation;

public class FluentValidationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ru");
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidatior>();
    }
}