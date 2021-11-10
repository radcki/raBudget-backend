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
using raBudget.Common.Query;
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
            services.AddDbContext<IWriteDbContext, WriteDbContext>(options =>
                                                                   {
                                                                       options.UseMySql(mysqlConnectionString,
                                                                                        MariaDbServerVersion.LatestSupportedServerVersion,
                                                                                        builder => { builder.MigrationsAssembly("raBudget.Api"); });
                                                                   }, ServiceLifetime.Transient);
            services.AddScoped<IReadDbContext, ReadDbContext>(); 
            services.AddTransient<AccessControlService>();
            services.AddTransient<BalanceService>();

            services.AddIdentityServices(Configuration);

            services.AddMediatR(applicationAssembly, apiAssembly);
            services.AddTransient<WriteDbContext>();

            services.AddSignalR();
            services.AddTransient<TransactionNotificationsHub>();
            services.AddTransient<BalanceNotificationsHub>();

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

            services.AddControllers(options =>
                                    {
                                        options.ModelBinderProviders.Insert(0, new ModelBinderProvider());
                                    })
                    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });

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
            app.UsePathBase("/api");

            if (env.IsDevelopment())
            {
                app.UseForwardedHeaders();
                app.UseSwagger(options =>
                               {
                                   options.RouteTemplate = "swagger/{documentName}/swagger.json";
                                   options.SerializeAsV2 = true;
                               });
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "raBudgetApi"));
                app.UseCors(builder => builder.WithOrigins("https://localhost:8080")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials());
            }
            else
            {
                app.UseCors(builder => builder.WithOrigins("https://budget.rabt.pl")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials());
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServices();
            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllers();
                                 endpoints.MapHub<BalanceNotificationsHub>("hubs/balance-notifications");
                                 endpoints.MapHub<TransactionNotificationsHub>("hubs/transaction-notifications");
                             });
        }
    }
}