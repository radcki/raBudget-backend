using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using raBudget.Api.Infrastructure;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Infrastructure.Database;

namespace raBudget.Api
{
    public static partial class StartupExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            //var identityService = services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            //                                                                          {
            //                                                                              o.Stores.MaxLengthForKeys = 128;
            //                                                                              o.SignIn.RequireConfirmedAccount = true;
            //                                                                              o.User.RequireUniqueEmail = true;

            //                                                                              o.Password.RequireDigit = true;
            //                                                                              o.Password.RequiredLength = 6;
            //                                                                              o.Password.RequireNonAlphanumeric = false;
            //                                                                              o.Password.RequireUppercase = false;
            //                                                                          })
            //                              .AddEntityFrameworkStores<WriteDbContext>()
            //                              .AddDefaultTokenProviders();

            //identityService.Services.TryAddTransient<IEmailSender, EmailSender>();


            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    .AddJwtBearer(ConfigureJwtBearer)
                    .AddOAuth2Introspection("introspection", options =>
                                                             {
                                                                 options.Authority = "https://auth.rabt.pl";
                                                                 options.ClientId = "rabudget";
                                                             });
            
            services.AddHttpContextAccessor();
            services.AddTransient<IUserContext, UserContext>();
        }

        public static void UseIdentityServices(this IApplicationBuilder app)
        {
            //app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.Authority = "https://auth.rabt.pl";
            options.Audience = "https://auth.rabt.pl/resources";
            //options.Audience = Configuration["Authentication:Audience"];

            options.TokenValidationParameters = new TokenValidationParameters()
                                                {
                                                    ValidateLifetime = true,
                                                    ValidateIssuer = true,
                                                    ValidateAudience = true,
                                                    ValidateIssuerSigningKey = true,
                                                    ValidTypes = new[] {"at+jwt"}
                                                };

            options.RequireHttpsMetadata = false;
            options.Events = new JwtBearerEvents
                             {
                                 OnAuthenticationFailed = c =>
                                                          {
                                                              //ProblemDetails problem;
                                                              //if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                                              //{
                                                              //    problem = new ProblemDetails()
                                                              //    {
                                                              //        Title = "Token has expired",
                                                              //        Status = StatusCodes.Status401Unauthorized,
                                                              //        Detail = Environment.IsDevelopment()
                                                              //                               ? c.Exception.Message
                                                              //                               : null
                                                              //    };
                                                              //}
                                                              //else
                                                              //{
                                                              //    problem = new ExceptionProblemDetails(c.Exception);
                                                              //}

                                                              //c.NoResult();
                                                              //c.Response.StatusCode = 401;
                                                              //c.Response.ContentType = "application/json";
                                                              //return c.Response.WriteAsync(JsonConvert.SerializeObject(problem));
                                                              return Task.CompletedTask;
                                                          },
                                 OnMessageReceived = context =>
                                                     {
                                                         var accessToken = context.Request.Query["access_token"];

                                                         var path = context.HttpContext.Request.Path;
                                                         if (path.StartsWithSegments("/hubs"))
                                                         {
                                                             context.Token = accessToken;
                                                         }

                                                         return Task.CompletedTask;
                                                     },
                             };

            options.SaveToken = true;

        }
    }
}