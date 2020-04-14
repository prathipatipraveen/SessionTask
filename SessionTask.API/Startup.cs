using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SessionTask.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SessionTask.Models.Helpers;
using SessionTask.DataAccess.Entities;
using SessionTask.DataAccess.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using SessionTask.Infrastructure.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HealthChecks.UI.Client;

namespace SessionTask.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
            });
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddDbContext<SessionTaskContext>(
                options => options.UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetValue<string>("SqlServerConnectionString")));

            //services.AddDbContext<SessionTaskContext>(opt =>
            //    opt.UseInMemoryDatabase("SessionTaskDb"));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddScoped<ISessionTaskRepository, SessionTaskRepository>();
            var connectionString = Configuration.GetValue<string>("SqlServerConnectionString");
            services.AddHealthChecks()
                .AddSqlServer(connectionString, failureStatus: HealthStatus.Unhealthy,tags: new[] {"ready" });

            services.AddHealthChecksUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Exception Handling middleware
            app.UseApiExceptionHandler(options =>
            {
                options.AddResponseDetails = UpdateApiErrorResponse;
                options.DetermineLogLevel = DetermineLogLevel;
            });

            app.UseHttpsRedirection();//To force https connection
            app.UseRouting();

            //Allow requests for only authorized urls
            //app.UseCors(x => x
            //    .WithOrigins(Configuration.GetValue<string>("WebAppURL"))
            //    .AllowAnyHeader()
            //    .AllowAnyMethod()
            //);

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseAuthorization();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions() { 
                ResultStatusCodes = { 
                    [HealthStatus.Healthy]=StatusCodes.Status200OK,
                    [HealthStatus.Degraded]=StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy]=StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = WriteHealthCheckReadyResponse,
                    Predicate = (check) => check.Tags.Contains("ready")
                }).RequireAuthorization();
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    ResultStatusCodes = {
                    [HealthStatus.Healthy]=StatusCodes.Status200OK,
                    [HealthStatus.Degraded]=StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy]=StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = WriteHealthCheckLiveResponse,
                    Predicate = (check) => !check.Tags.Contains("ready")
                }).RequireAuthorization();
                endpoints.MapHealthChecks("/healthui", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
            app.UseHealthChecksUI();
        }

        private Task WriteHealthCheckLiveResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
                new JProperty("OverallStatus", result.Status.ToString()),
                new JProperty("Total Duraton", result.TotalDuration.TotalSeconds.ToString("0:0.00"))
                );
            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }

        private Task WriteHealthCheckReadyResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
                new JProperty("OverallStatus", result.Status.ToString()),
                new JProperty("Total Duraton", result.TotalDuration.TotalSeconds.ToString("0:0.00")),
                new JProperty("DependencyHealthChecks", new JObject(result.Entries.Select(dicItem =>
                 new JProperty(dicItem.Key, new JObject(
                     new JProperty("Status", dicItem.Value.Status.ToString())
                     ))
                )))
                );
            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }

        private LogLevel DetermineLogLevel(Exception ex)
        {
            if (ex.Message.StartsWith("Database", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.StartsWith("Network-Error", StringComparison.InvariantCultureIgnoreCase))
            {
                return LogLevel.Critical;
            }
            return LogLevel.Error;
        }

        private void UpdateApiErrorResponse(HttpContext context, Exception ex, ApiError error)
        {
            if (ex.GetType().Name == typeof(SqlException).Name)
            {
                error.Detail = "Database Exception!";
            }
        }

    }
}
