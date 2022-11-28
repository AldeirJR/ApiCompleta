using DevJr.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevJr.Api.Configuration
{
    public static class ApiConfig
    {

        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;   
            } );

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;   


            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                  builder => builder.SetIsOriginAllowed(_ => true)

                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials()
                                          

                                  );

                options.AddPolicy("Production",
                              builder => builder.WithMethods("GET","PUT","DELETE","POST")
                              .WithOrigins("http://localhost:4200/")
                              .SetIsOriginAllowedToAllowWildcardSubdomains()
                              .AllowAnyHeader()

                              );


            });

            return services;
        }

        public static IApplicationBuilder UseApiConfig(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors(); // Usar apenas nas demos => Configuração Ideal: Production
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

         

            return app;
        }


    }
}
