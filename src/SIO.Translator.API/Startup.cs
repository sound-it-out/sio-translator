using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.EntityFrameworkCore.SqlServer;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Translator.Infrastructure.AWS.Extensions;
using SIO.Translator.Infrastructure.Extensions;
using SIO.Translator.Infrastructure.Google.Extensions;
using SIO.Translator.Migrations.DbContexts;

namespace SIO.Translator.API
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IHostEnvironment env,
            IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure();

            var openEventSourceBuilder = services.AddOpenEventSourcing()
                .AddEntityFrameworkCoreSqlServer(options => {
                    options.MigrationsAssembly("SIO.Translator.Migrations");
                })
                .AddCommands()
                .AddEvents()
                .AddJsonSerializers();

            if (_env.IsDevelopment())
            {
                services.AddLocalFiles()
                    .AddLocalTranslations();

                openEventSourceBuilder.AddRabbitMq(options =>
                 {
                     options.UseConnection(_configuration.GetValue<string>("RabbitMQ:Connection"))
                         .UseExchange(e =>
                         {
                             e.WithName(_configuration.GetValue<string>("RabbitMQ:Exchange:Name"));
                             e.UseExchangeType(_configuration.GetValue<string>("RabbitMQ:Exchange:Type"));
                         })
                         .UseManagementApi(m =>
                         {
                             m.WithEndpoint(_configuration.GetValue<string>("RabbitMQ:ManagementApi:Endpoint"));
                             m.WithCredentials(_configuration.GetValue<string>("RabbitMQ:ManagementApi:Username"), _configuration.GetValue<string>("RabbitMQ:ManagementApi:Password"));
                         });
                 });
            }
            else
            {
                services.AddGoogleTranslations()
                    .AddAWSTranslations()
                    .AddAWSFiles();
            }

            services.AddDbContext<SIOTranslatorDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("Translator"), sql =>
                {
                    sql.EnableRetryOnFailure();
                    sql.MigrationsAssembly("SIO.Translator.Migrations");
                }));

            services.AddHangfire(options => options.UseSqlServerStorage(_configuration.GetConnectionString("Translator")));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }
    }
}
