using api.basket.Configuration;
using api.basket.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net;

namespace api.basket.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterBasketApi(this IServiceCollection services, IConfiguration configuration)
        {
            Cors(services);

            Iis(services);

            Automapper(services);

            Swagger(services);

            Basket(services, configuration);

        }

        private static void Basket(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BasketStorageSettings>(configuration.GetSection(nameof(BasketStorageSettings)));

            services.AddSingleton<IBasketStorageSettings>(sp => sp.GetRequiredService<IOptions<BasketStorageSettings>>().Value);

            services.AddSingleton<BasketService>();
        }

        private static void Swagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket api", Version = "v1" });
            });
        }

        private static void Automapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }

        private static void Iis(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        private static void Cors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        public static void ConfigureBasketApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog web app V1");
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
