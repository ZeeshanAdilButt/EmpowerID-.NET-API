using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ActivityApp.Application.ServiceRegistrations;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL;
using ActivityApp.Infrastructure.SQL.Common;
using ActivityApp.Infrastructure.SQL.RepositoryRegistrations;
using ActivityApp.ServiceRegistrations;
using System.Reflection;
using WatchDog;
using System.Data.SqlClient;

namespace ActivityApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public bool IsSqlServerExists { get; set; }

        private bool IsSqlServerPresent()
        {
            // Check if SQL Server is present
            try
            {
                using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //// ********************
            //// Setup CORS
            //// ********************
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            //corsBuilder.AllowAnyOrigin(); // For public access.
            corsBuilder.WithOrigins("http://localhost:3000", ""); // for a specific url. Don't add a forward slash on the end!
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

            services.AddCors();

            //TODO: add context
            DbContextOptionsBuilder<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseLazyLoadingProxies();
            Common.options = options;

            IsSqlServerExists = IsSqlServerPresent();

            if (IsSqlServerExists)
            {
                // Use SQL Server
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                services.AddDbContext<ApplicationDbContext>(
                      options => options.UseSqlServer(
                          Configuration.GetConnectionString("DefaultConnection"),
                          b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }
            else
            {
                // Use in-memory database
                options.UseInMemoryDatabase("MyInMemoryDatabase");
                
                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("MyInMemoryDatabase")
                );

            }


            //services.AddDbContext<ApplicationDbContext>(opt =>
            //{
            //    opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sql => sql.MigrationsAssembly(assembly));
            //});
            //services.AddScoped<SignInManager<IdentityUser>>();
            //services.AddScoped<RoleManager<IdentityRole>>();
            //services.AddScoped<UserManager<IdentityUser>>();

            //services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlServer(
            //       Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AspNetUsers, AspNetRoles>()
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();

            //TODO: register with application db context
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<loopvineContextBaseDontUse>()
            //    .AddDefaultTokenProviders();

            services.RegisterJWTAuthenticationService(Configuration);

            services.InstallSwagger(Environment, Configuration);


            services.AddAutoMapper(typeof(Startup));
            services.AddMvc(properties =>
            {
                //properties.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
            });
            services.AddCors();

            //services.AddSignalR();
            services.RegisterCommon();

            services.RegisterRepositories();

            services.RegisterAPIServices();
            //services.RegisterServiceProviders();

            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                opt.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            });

            if (IsSqlServerExists)
            {
                services.AddWatchDogServices(opt =>
                {
                    opt.SqlDriverOption = WatchDog.src.Enums.WatchDogSqlDriverEnum.MSSQL;
                    opt.SetExternalDbConnString = Configuration.GetConnectionString("DefaultConnection");
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    context.Database.Migrate();
            //}
            //else
            //{
            if (IsSqlServerExists)
                context.Database.Migrate();

            app.UseHsts();

            //}

            app.UseHttpsRedirection();

            app.UseSwaggerConfig();

            app.UseCors("SiteCorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            if (IsSqlServerExists)
            {
                app.UseWatchDogExceptionLogger();

                app.UseWatchDog(conf =>
                {
                    conf.WatchPageUsername = Configuration["WatchDog:UserName"];
                    conf.WatchPagePassword = Configuration["WatchDog:Password"];
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
