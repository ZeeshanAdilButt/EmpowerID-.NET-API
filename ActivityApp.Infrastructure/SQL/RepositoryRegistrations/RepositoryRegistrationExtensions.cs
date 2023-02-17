using Microsoft.Extensions.DependencyInjection;
using ActivityApp.Infrastructure.SQL.Repostiories.Implementations;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
namespace ActivityApp.Infrastructure.SQL.RepositoryRegistrations
{
    public static class RepositoryRegistrationExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            //TODO: register repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
        }
    }
}
