using Daryaftyar.Application;
using Daryaftyar.Infrastructure;
using Daryaftyar.Infrastructure.Persistence;
using Serilog;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseInfrastructure(configuration, app.Environment);

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MigrateDatabase();

app.Run();