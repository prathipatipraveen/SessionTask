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
using SessionTask.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using SessionTask.Infrastructure.Middleware;

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

            app.UseHttpsRedirection();
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
                endpoints.MapControllers();
            });

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

        private static void AddTestData(SessionTaskContext context)
        {
            context.User.AddRange(
                new List<User>
                {
                     new User
                    {
                        UserId = 1,
                        UserName = "admin",
                        Password = "admin",
                        IsActive = true
                    },
                    new User
                    {
                        UserId = 2,
                        UserName = "attendee",
                        Password = "attendee",
                        IsActive = true
                    },
                    new User
                    {
                        UserId = 3,
                        UserName = "Host",
                        Password = "host",
                        IsActive = true
                    }
            });

            context.Role.AddRange(
                new List<Role>
                {
                     new Role
                    {
                        RoleId = 1,
                        RoleName = "admin"
                    },
                    new Role
                    {
                        RoleId = 2,
                        RoleName = "attendee"
                    },
                    new Role
                    {
                        RoleId = 3,
                        RoleName = "Host"
                    }
            });
        }
    }
}
