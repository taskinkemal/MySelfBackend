﻿using System;
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
using Newtonsoft.Json;
using DataLayer.Managers.BadgeCheckers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddCors(o => o.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            }));

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                }).AddMvcOptions(options => {
                    options.EnableEndpointRouting = false;
                });

            //services.AddDbContext<MyselfContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            if (Configuration.GetSection("AppSettings").GetValue<bool>("IsSqLite")) 
                services.AddDbContext<MyselfContext>(
                    options => options.UseSqlite($"Data Source={AppContext.BaseDirectory}/myself.db"), ServiceLifetime.Scoped);
            else
                services.AddDbContext<MyselfContext>(
                    options => options.UseSqlServer(Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString")), ServiceLifetime.Scoped);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.Configure<IISServerOptions>(options =>
            {
                // The Newtonsoft.json is not working without allowing synchronous IO.
                // And at the moment we cannot switch to System.Text.Json because the GraphQl libraries are still using Newtonsoft.
                // https://github.com/dotnet/aspnetcore/issues/8302
                options.AllowSynchronousIO = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                // The Newtonsoft.json is not working without allowing synchronous IO.
                // And at the moment we cannot switch to System.Text.Json because the GraphQl libraries are still using Newtonsoft.
                // https://github.com/dotnet/aspnetcore/issues/8302
                options.AllowSynchronousIO = true;
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                SetNewtonsoftSerializerSettings(options.SerializerSettings);
            });

            InjectDependencies(services);
        }

        private void InjectDependencies(IServiceCollection services)
        {
            services.AddSingleton<ILogManager, LogManager>();

            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<ITaskManager, TaskManager>();
            services.AddScoped<IEntryManager, EntryManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IGoalManager, GoalManager>();

            services.AddScoped<IBadgeCheck, DataEntryCheck>();
            services.AddScoped<IBadgeCheck, SilverGoalCheck>();
            services.AddScoped<IBadgeCheck, GoldenGoalCheck>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        private void SetNewtonsoftSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            serializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            serializerSettings.ContractResolver = new DefaultContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
