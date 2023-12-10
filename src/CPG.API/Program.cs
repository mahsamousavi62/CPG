using CPG.API.Helper;
using CPG.Application;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure;
using CPG.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
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

builder.Services
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

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

var supportedLanguages = new List<CultureInfo>
    {
        new("fa")
    };

builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    opt.DefaultRequestCulture = new RequestCulture("fa", "fa");
    opt.SupportedCultures = supportedLanguages;
    opt.SupportedUICultures = supportedLanguages;
});

var app = builder.Build();

app.UseCors(DefaultCorsPolicyName);

if (Convert.ToBoolean(configuration["EnableSwagger"]))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedLanguages,
    SupportedUICultures = supportedLanguages
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseInfrastructure(configuration, app.Environment);

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.MigrateDatabase();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();