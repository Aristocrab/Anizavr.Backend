﻿using Microsoft.AspNetCore.Builder;

namespace AspNetCore.AppModules;

public interface IAppModule
{
    void ConfigureServices(WebApplicationBuilder builder);

    void ConfigureApplication(WebApplication app);
    
    bool Enabled { get; set; }
    
    int OrderIndex { get; set; }
}