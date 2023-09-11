using System.Globalization;
using AspNetCore.Extensions.AppModules;
using Anizavr.Backend.Application.Validators;
using AspNetCore.Extensions.AppModules.ModuleTypes;
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
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
    }
}