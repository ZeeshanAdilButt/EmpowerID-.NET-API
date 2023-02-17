using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ActivityApp.Options;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace ActivityApp.ServiceRegistrations
{

    public static class APIServiceRegistrationExtensions
    {
        private const string SwaggerPath = "/swagger/v1/swagger.json";

        /// <summary>
        /// UseCorsMiddleware registers UseCors.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void RegisterCommon(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// RegisterJWTAuthenticationService registers common Authentication for JWT.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterJWTAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }

        /// <summary>
        /// UseCorsMiddleware registers UseCors.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder app)
        {
            return app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
        }

        /// <summary>
        /// RegisterSwagger registers Swagger components.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static void InstallSwagger(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Employee App",
                    Description = "Employee App"
                });
            });
        }

        /// <summary>
        /// UseSwaggerMiddleware registers Swagger Middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app, string applicationName)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            return app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(SwaggerPath, applicationName);
            });
        }

        public static void UseSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee App");
                    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                    c.DisplayRequestDuration();
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.ShowExtensions();
                }
            );

        }
    }
}
