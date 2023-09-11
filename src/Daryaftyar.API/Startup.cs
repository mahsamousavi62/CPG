using Daryaftyar.Application;
using Daryaftyar.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Daryaftyar.API
{
    public class Startup
    {
        private readonly bool enableSwagger;
        private const string ApiVersion = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _ = bool.TryParse(Configuration["App:EnableSwagger"], out enableSwagger);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();

            services.AddInfrastructure(Configuration);
            services.AddApplication(Configuration);

            if (enableSwagger)
            {
                _ = services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(ApiVersion, new OpenApiInfo
                    {
                        Version = ApiVersion,
                        Title = "Charispay API",
                        Description = "Charispay",
                        Contact = new OpenApiContact
                        {
                            Name = "Charispay",
                            Email = string.Empty,
                            Url = new Uri("https://daryaftyar.charisma.ir"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://daryaftyar.charisma.ir"),
                        },
                    });
                    options.DocInclusionPredicate((docName, description) => true);

                    // Define the BearerAuth scheme that's in use
                    options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    });
                });

                _ = services.AddSwaggerGenNewtonsoftSupport();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseInfrastructure(Configuration, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("api/healthz");
            });
        }
    }
}
