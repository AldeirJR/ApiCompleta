using DevJr.Api.Extensions;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DevJr.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "a4a529fa15e549919c4048fffc8f9c13";
                o.LogId = new Guid("9463109d-a963-4a7f-b96e-920341d12fda");
            });


            //services.AddLogging(  builder =>
            //{
            //    builder.Services.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "a4a529fa15e549919c4048fffc8f9c13";
            //        o.LogId = new Guid("9463109d-a963-4a7f-b96e-920341d12fda");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Information);
            //} );

            services.AddHealthChecks()
                .AddElmahIoPublisher(options =>
                {
                    options.ApiKey = "a4a529fa15e549919c4048fffc8f9c13";
                    options.LogId = new Guid("9463109d-a963-4a7f-b96e-920341d12fda");
                    options.HeartbeatId = "API Fornecedores";

                })
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            //services.AddHealthChecksUI();
               

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            app.UseHealthChecks("/api/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

            });
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/api/hc-ui";


            });


            return app;
        }
    }
}
