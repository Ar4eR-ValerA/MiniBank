using System.Text.Json.Serialization;
using MiniBank.Core;
using MiniBank.Data;
using MiniBank.Web.Middlewares;

namespace MiniBank.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddData(_configuration);
            services.AddCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<UserFriendlyExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}