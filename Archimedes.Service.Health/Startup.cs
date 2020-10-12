using Archimedes.Library.Domain;
using Archimedes.Service.Health.Hubs;
using Archimedes.Service.Ui.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Archimedes.Service.Health
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
            services.AddHttpClient<IHttpBrokerClient, HttpBrokerClient>();
            services.AddHttpClient<IHttpCandleClient, HttpCandleClient>();
            services.AddHttpClient<IHttpRepositoryClient, HttpRepositoryClient>();
            services.AddHttpClient<IHttpRepositoryApiClient, HttpRepositoryApiClient>();
            services.AddHttpClient<IHttpUiClient, HttpUiClient>();
            services.AddHttpClient<IHttpStrategyClient, HttpStrategyClient>();


            services.AddHttpClient<IHttpRabbitClient, HttpRabbitClient>();

            services.AddSignalR();
            
            services.AddTransient<IHealthResponse, HealthResponse>();

            services.AddSingleton<IHealthDataStore, HealthDataStore>();
            services.AddLogging();
            services.Configure<Config>(Configuration.GetSection("AppSettings"));
            services.AddControllers();

            services.AddHostedService<HealthServiceUi>();
            services.AddHostedService<HealthServiceBroker>();
            services.AddHostedService<HealthServiceCandle>();
            services.AddHostedService<HealthServiceRepository>();
            services.AddHostedService<HealthServiceRepositoryApi>();
            services.AddHostedService<HealthServiceRabbit>();
            services.AddHostedService<HealthServiceStrategy>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", x =>
                {
                    x.WithOrigins("http://localhost:4200",
                            "http://angular-ui.dev.archimedes.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
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

            app.UseCors("AllowAny");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HealthHub>("/Hubs/Health");
            });
        }
    }
}
