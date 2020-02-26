using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
//using RabbitMQAdapter;
using RealTimeChatApp.Controllers;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.RabbitMQ;
using WebSocketManager;

namespace RealTimeChatApp
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
            services.AddWebSocketManager();
            //services.AddRazorPages();

            services.AddSingleton<IRabbitWork, RabbitWork>(sp =>
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = Configuration["RabbitMqConnection:HostName"],
                    //Port = Convert.ToInt32(Configuration["RabbitMqConnection.Port"]),
                    UserName = Configuration["RabbitMqConnection:Username"],
                    Password = Configuration["RabbitMqConnection:Password"],
                    VirtualHost = Configuration["RabbitMqConnection:VirtualHost"]
                };
                return new RabbitWork(connectionFactory.CreateConnection());
            });

            //services.AddOptions();
            services.AddTransient<IDataProvider, DataProvider>();
        



        //services.AddSingleton<IRabbitWork, RabbitWork>();
        //services.AddSingleton<IRabbitWork, RabbitWork>();

            services.AddMvc(); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseWebSockets();
            
            app.MapWebSocketManager("/chat", serviceProvider.GetService<ChatHandler>());

            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
