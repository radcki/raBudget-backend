using System;
using System.Globalization;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using raBudget.Api.Infrastructure;
using raBudget.Domain.Interfaces;
using raBudget.Infrastructure.Database;
using raBudget.Infrastructure.Services;

namespace raBudget.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var applicationAssembly = Assembly.Load("raBudget.Application");

            services.Configure<ApiConfiguration>(Configuration.GetSection("SystemConfiguration"));
            services.AddDbContext<IWriteDbContext, WriteDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql"), builder => { builder.MigrationsAssembly("raBudget.Infrastructure"); }));

            services.AddIdentityServices(Configuration);

            services.AddMediatR(applicationAssembly);
            services.AddTransient<WriteDbContext>();
            services.AddTransient<IEmailClient, EmailClient>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddDataAnnotationsLocalization();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new[] { "pl", "en" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                                                                      .AddSupportedCultures(supportedCultures)
                                                                      .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyHeader()
                                          .AllowAnyMethod()
                                          .AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServices();
            //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllerRoute(name: "default",
                                                              pattern: "{controller=Home}/{action=Index}/{id?}");
                             });

        }
    }
}