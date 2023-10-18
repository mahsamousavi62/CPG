using CPG.Application;
using CPG.Infrastructure;
using CPG.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var configuration = builder.Configuration;

builder.Services
    .AddInfrastructure(configuration)
    .AddApplication(configuration);

builder.Host.UseSerilog((context, configuation) =>
    configuation.ReadFrom.Configuration(context.Configuration));

builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    var supportedLanguages = new List<CultureInfo> {
                    new CultureInfo("en"),
                    new CultureInfo("fa")
                };

    opt.DefaultRequestCulture = new RequestCulture("fa", "fa");    
    opt.SupportedCultures = supportedLanguages;
    opt.SupportedUICultures = supportedLanguages;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseInfrastructure(configuration, app.Environment);

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.MigrateDatabase();

app.Run();