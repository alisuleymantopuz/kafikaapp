using api.orders.Configuration;  
using api.orders.Messaging.Sender;
using api.orders.persistence.Extensions;
using api.orders.persistence.Messaging.Sender;
using api.orders.persistence.Services;
using api.orders.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System; 
using System.Net;
using System.Text; 

namespace api.orders.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterOrdersApi(this IServiceCollection services, IConfiguration configuration)
        {
            Cors(services);
            IIS(services);
            Swagger(services);
            services.RegisterOrdersDataContext(configuration);
            Jwt(services, configuration);
            RabbitMq(services, configuration);
        }

        private static void RabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var serviceClientSettingsConfig = configuration.GetSection("RabbitMq"); 
            services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);
            services.AddSingleton<IOrderCreateMessagingSender, OrderCreateMessagingSender>();
            services.AddSingleton<IOrderShipperMessagingSender, OrderShipperMessagingSender>();
            services.AddSingleton<IOrderDeliveryMessagingSender, OrderDeliveryMessagingSender>();
        }

        private static void Jwt(IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<IUserService, UserService>();
        }

        private static void Cors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        private static void IIS(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        private static void Swagger(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders api", Version = "v1" });
            });
        }

        public static void ConfigureOrdersApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders web api V1");
            });

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }
}
