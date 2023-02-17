using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ActivityApp.Application.Implementations;
using ActivityApp.Application.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.ServiceRegistrations
{
    public static class ServiceRegistrationExtensions
    {
        private const string SwaggerPath = "/swagger/v1/swagger.json";

        public static void RegisterAPIServices(this IServiceCollection services)
        {
            //TODO: add services
            //services.AddTransient<ICommonService, CommonService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
        }

        public static void RegisterServiceProviders(this IServiceCollection services)
        {
            //TODO: add services
           
        }

    }
}
