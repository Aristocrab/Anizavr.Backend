using System.Globalization;
using Anizavr.Backend.Application.Validators;
using Calabonga.AspNetCore.AppDefinitions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Definitions.FluentValidation;

public class FluentValidationDefinition : AppDefinition
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