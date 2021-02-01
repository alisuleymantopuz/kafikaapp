using api.products.Configuration;
using api.products.persistence.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;

namespace api.products.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterProductsApi(this IServiceCollection services, IConfiguration configuration)
        {
            Cors(services);
            IIS(services);
            Swagger(services);
            services.RegisterProductsDataContext(configuration);
            RabbitMq(services, configuration);
        }

        private static void RabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var serviceClientSettingsConfig = configuration.GetSection("RabbitMq");
            var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();
            services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);
            //services.AddSingleton<IOrderCreateMessagingSender, OrderCreateMessagingSender>();
            //services.AddTransient<IOrderStatusUpdateService, OrderStatusUpdateService>();

            //if (serviceClientSettings.Enabled)
            //{
            //    services.AddHostedService<OrderCreateMessagingReceiver>();
            //}
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products api", Version = "v1" });
            });
        }

        public static void ConfigureProductsApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products web api V1");
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
