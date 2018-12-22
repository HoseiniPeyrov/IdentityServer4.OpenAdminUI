// 
//  Startup.cs
//  Copyright (c) Johan Boström. All rights reserved.
//  Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
//  

using System;
using System.Reflection;
using IdentityServer4.OpenAdminUI.Core.Stores;
using IdentityServer4.OpenAdminUI.EntityFramework.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace IdentityServer4.OpenAdminUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            #region IdentityServer

            var connectionString = Configuration.GetConnectionString("IdentityServer");
            var migrationsAssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            void ContextOptionsBuilder(DbContextOptionsBuilder options)
            {
                // Configure database
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssemblyName));
            }

            // Add identitycontext
            //services.AddDbContext<IdentityDbContext>(ContextOptionsBuilder);

            // Configure identity with EF
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<IdentityDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddScoped<IAdminClientStore, AdminClientStore>();

            var identityServerBuilder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddConfigurationStore(options => { options.ConfigureDbContext = ContextOptionsBuilder; })
                .AddOperationalStore(options => { options.ConfigureDbContext = ContextOptionsBuilder; });
            //.AddAspNetIdentity<IdentityUser>();
            // this is something you will want in production to reduce load on and requests to the DB
            //.AddConfigurationStoreCache();

            if (Environment.IsDevelopment())
                identityServerBuilder.AddDeveloperSigningCredential();
            else
                throw new Exception("need to configure key material");

            #endregion

            services
                .AddAuthentication();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Open Admin UI",
                    Version = "v1"
                });
            });


            return services.BuildServiceProvider(true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                })
                .UseStaticFiles()
                .UseMvcWithDefaultRoute();
        }
    }
}