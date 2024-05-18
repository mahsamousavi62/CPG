using CPG.API.Helper;
using CPG.API.Helper.Localization;
using CPG.Application;
using CPG.Application.Shared;
using CPG.Infrastructure;
using CPG.Infrastructure.Persistence;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{

    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddEnumsWithValuesFixFilters();
    opt.SchemaFilter<EnumerationToEnumSchemaFilter>();

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();
builder.Services
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

builder.Host.UseSerilog((context, configuation) =>
    configuation.ReadFrom.Configuration(context.Configuration));

const string DefaultCorsPolicyName = "localhost";

var corsOrigins = configuration["CorsOrigins"]!
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .ToArray();
var supportedLanguages = new List<CultureInfo>
    {
        new("en"),
        new("fa")
    };

builder.Services.AddCors(
                options => options.AddPolicy(
                    DefaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials()));

builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    opt.SupportedCultures = supportedLanguages;
    opt.SupportedUICultures = supportedLanguages;
});

var app = builder.Build();

app.UseCors(DefaultCorsPolicyName);
app.MigrateDatabase();

if (Convert.ToBoolean(configuration["EnableSwagger"]))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}



app.UseRequestLocalization(new RequestLocalizationOptions
{
    SupportedCultures = supportedLanguages,
    SupportedUICultures = supportedLanguages
});

app.MapHub<NotificationHub>("/Notify");
app.UseHttpsRedirection();
app.UseRouting();
app.UseFileServer();
app.UseStaticFiles();
app.UseInfrastructure(configuration, app.Environment);
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.MapGraphQL();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();