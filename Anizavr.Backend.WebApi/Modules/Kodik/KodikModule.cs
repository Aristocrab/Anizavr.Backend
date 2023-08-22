﻿using Anizavr.Backend.Application.KodikApi;
using Anizavr.Backend.WebApi.Modules.Shared;
using Refit;

namespace Anizavr.Backend.WebApi.Modules.Kodik;

public class KodikModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(RestService.For<IKodikApi>("https://kodikapi.com"));
    }
}