using api.orders.persistence.Extensions;
using api.orders.persistence.Messaging.Sender;
using api.orders.persistence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace api.orders.receivers.created
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
            services.RegisterOrdersDataContext(Configuration);

            var serviceClientSettingsConfig = Configuration.GetSection("RabbitMq");
            var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();
            services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig); 
            services.AddTransient<IOrderStatusUpdateService, OrderStatusUpdateService>();
            services.AddSingleton<IOrderProductsUpdateMessagingSender, OrderProductsUpdateMessagingSender>();
            services.AddSingleton<IRevertProductStocksMessagingSender, RevertProductStocksMessagingSender>();
            services.AddSingleton<IEmailMessagingSender, EmailMessagingSender>();
            services.AddSingleton<ISmsMessagingSender, SmsMessagingSender>();

            if (serviceClientSettings.Enabled)
            {
                services.AddHostedService<OrderCreateMessagingReceiver>();
            }

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
