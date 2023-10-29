using CPG.Application;
using CPG.Infrastructure;
using CPG.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
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

const string DefaultCorsPolicyName = "localhost";

builder.Services.AddCors(
                options => options.AddPolicy(
                    DefaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(configuration["CorsOrigins"]!
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .ToArray()!)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials()));
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

app.UseCors(DefaultCorsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.UseInfrastructure(configuration, app.Environment);

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.MigrateDatabase();

app.Run();