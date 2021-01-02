using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using raBudget.Api.Hubs;
using raBudget.Api.Infrastructure;
using raBudget.Application.Infrastructure;
using raBudget.Common;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Infrastructure.Database;

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
            var apiAssembly = Assembly.Load("raBudget.Api");
            var applicationAssembly = Assembly.Load("raBudget.Application");

            services.Configure<ApiConfiguration>(Configuration.GetSection("SystemConfiguration"));
            var mysqlConnectionString = Configuration.GetConnectionString("MySql");
            services.AddDbContext<IWriteDbContext, WriteDbContext>(options => options.UseMySql(mysqlConnectionString, 
                                                                                               MariaDbServerVersion.LatestSupportedServerVersion,
                                                                                               builder =>
                                                                                               {
                                                                                                   builder.MigrationsAssembly("raBudget.Api");
                                                                                               }));
            services.AddTransient<IReadDbContext, ReadDbContext>();
            services.AddTransient<AccessControlService>();
            services.AddTransient<BalanceService>();

            services.AddIdentityServices(Configuration);

            services.AddMediatR(applicationAssembly, apiAssembly);
            services.AddTransient<WriteDbContext>();

			services.AddSignalR();

            services.AddAutoMapper(applicationAssembly, apiAssembly);
            var config = new MapperConfiguration(cfg =>
                                                 {
                                                     cfg.AddProfile(new ApiAutoMapperProfile());
                                                     cfg.AddProfile(new ApplicationAutoMapperProfile());
                                                 });
            services.AddSingleton(config);

            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(options =>
                                   {
                                       options.SwaggerDoc("v1", new OpenApiInfo() {Title = "raBudgetApi", Version = "v1"});
                                       options.CustomSchemaIds(x => x.FullName);
                                   });

            services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = KnownCulture.SupportedCultures().ToList();
            app.UseRequestLocalization(new RequestLocalizationOptions
                                       {
                                           SupportedCultures = supportedCultures,
                                           SupportedUICultures = supportedCultures,
                                           FallBackToParentCultures = true,
                                           FallBackToParentUICultures = true,
                                       });

            var forwardOptions = new ForwardedHeadersOptions
                                 {
                                     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                                     RequireHeaderSymmetry = false
                                 };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardOptions);

            if (env.IsDevelopment())
            {
                app.UseForwardedHeaders();
                app.UseSwagger(options =>
                               {
                                   options.RouteTemplate = "swagger/{documentName}/swagger.json";
                                   options.SerializeAsV2 = true;
                               });
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "raBudgetApi"));
            }
            else
            {
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");
            app.UseCors(builder => builder.AllowAnyHeader()
                                          .AllowAnyMethod()
                                          .AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServices();
            app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<BalanceNotificationsHub>("/balance-notifications-hub");
			});
        }
    }
}