using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common;
using Common.Interfaces;
using Common.Managers;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Managers;
using Newtonsoft.Json.Serialization;


namespace WebApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddCors(o => o.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            }));

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //services.AddDbContext<MyselfContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            if (Configuration.GetSection("AppSettings").GetValue<bool>("IsSqLite")) 
                services.AddDbContext<MyselfContext>(options => options.UseSqlite($"Data Source={AppContext.BaseDirectory}/myself.db"));
            else
                services.AddDbContext<MyselfContext>(options => options.UseSqlServer(Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString")));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InjectDependencies(services);
        }

        private void InjectDependencies(IServiceCollection services)
        {
            services.AddSingleton<ILogManager, LogManager>();

            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<ITaskManager, TaskManager>();
            services.AddScoped<IEntryManager, EntryManager>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("DefaultPolicy");
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "api/{controller}/{id?}");
            });

            if (Configuration.GetSection("AppSettings").GetValue<bool>("IsSqLite"))
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<MyselfContext>();
                    //context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
