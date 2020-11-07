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
            //services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            //                                                       {
            //                                                           options.SignIn.RequireConfirmedAccount = true;
            //                                                           options.User.RequireUniqueEmail = true;

            //                                                           options.Password.RequireDigit = true;
            //                                                           options.Password.RequiredLength = 6;
            //                                                           options.Password.RequireNonAlphanumeric = false;
            //                                                           options.Password.RequireUppercase = false;
            //                                                       })
            //        .AddRoles<ApplicationRole>()
            //        .AddEntityFrameworkStores<WriteDbContext>()
            //        .AddDefaultTokenProviders();

            //services.AddAuthentication(o =>
            //                           {
            //                               o.DefaultScheme = IdentityConstants.ApplicationScheme;
            //                               o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //                           })
            //        .AddIdentityCookies(o => { });

            var identityService = services.AddIdentity<ApplicationUser, IdentityRole>(o =>
                                                                                      {
                                                                                          o.Stores.MaxLengthForKeys = 128;
                                                                                          o.SignIn.RequireConfirmedAccount = true;
                                                                                          o.User.RequireUniqueEmail = true;

                                                                                          o.Password.RequireDigit = true;
                                                                                          o.Password.RequiredLength = 6;
                                                                                          o.Password.RequireNonAlphanumeric = false;
                                                                                          o.Password.RequireUppercase = false;
                                                                                      })
                                          .AddEntityFrameworkStores<WriteDbContext>()
                                          .AddDefaultTokenProviders();

            //identityService.AddSignInManager();
            identityService.Services.TryAddTransient<IEmailSender, EmailSender>();

            services.AddIdentityServer(options =>
                                       {
                                           options.Events.RaiseErrorEvents = true;
                                           options.Events.RaiseInformationEvents = true;
                                           options.Events.RaiseFailureEvents = true;
                                           options.Events.RaiseSuccessEvents = true;

                                           options.EmitStaticAudienceClaim = true;

                                           options.UserInteraction = new UserInteractionOptions()
                                                                     {
                                                                         //LogoutUrl = "/Identity/Logout",
                                                                         //LoginUrl = "/Identity/Login",
                                                                         //LoginReturnUrlParameter = "returnUrl",
                                                                         LogoutUrl = "/Identity/Logout",
                                                                         LoginUrl = "/Identity/Login",
                                                                         LoginReturnUrlParameter = "returnUrl",
                                           };

                                           //options.Authentication.CookieAuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                       })
                    .AddInMemoryClients(new[]
                                        {
                                            new Client
                                            {
                                                ClientId = "rabudget",
                                                RequireClientSecret = false,
                                                RequireConsent = false,
                                                AllowedGrantTypes = GrantTypes.Code,

                                                RedirectUris =
                                                {
                                                    "http://localhost:8080/auth/signinwin/main", 
                                                    "http://localhost:8080/auth/signinpop/main",
                                                    "https://localhost:8080/auth/signinwin/main", 
                                                    "https://localhost:8080/auth/signinpop/main"
                                                },
                                                PostLogoutRedirectUris = {"http://localhost:8080/"},

                                                AllowOfflineAccess = true,
                                                AllowedScopes =
                                                {
                                                    IdentityServerConstants.StandardScopes.OpenId,
                                                    IdentityServerConstants.StandardScopes.Profile,
                                                    IdentityServerConstants.StandardScopes.Email,
                                                    "rabudget"
                                                }
                                            },
                                        })
                    .AddInMemoryIdentityResources(new[]
                                                  {
                                                      new IdentityResources.OpenId(),
                                                      new IdentityResources.Profile(),
                                                      new IdentityResources.Email(),
                                                      new IdentityResource("rabudget", new[] {JwtClaimTypes.Audience}),
                                                  })
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddDeveloperSigningCredential();

            services.AddAuthentication(options =>
                                       {
                                           //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    //.AddJwtBearer(ConfigureJwtBearer)
                    .AddGoogle(options =>
                               {
                                   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                                   options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                                   options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
                                   options.ClaimActions.MapJsonKey(claimType: "role", jsonKey: "role");
                                   options.SaveTokens = true;
                                   options.Events.OnCreatingTicket = ctx =>
                                                                     {
                                                                         List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                                                                         tokens.Add(new AuthenticationToken()
                                                                                    {
                                                                                        Name = "TicketCreated",
                                                                                        Value = DateTime.UtcNow.ToString()
                                                                                    });

                                                                         ctx.Properties.StoreTokens(tokens);

                                                                         return Task.CompletedTask;
                                                                     };
                                   IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
                                   options.ClientId = googleAuthNSection["ClientId"];
                                   options.ClientSecret = googleAuthNSection["ClientSecret"];
                               });

            services.AddTransient<IUserContext, UserContext>();
        }

        public static void UseIdentityServices(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.Authority = "https://localhost:44393";
            options.Audience = "https://localhost:44393/resources";
            //options.Audience = Configuration["Authentication:Audience"];

            options.TokenValidationParameters = new TokenValidationParameters()
                                                {
                                                    ValidateLifetime = false,
                                                    ValidateIssuer = false,
                                                    ValidateAudience = false,
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
                                                     }
                             };

            options.SaveToken = true;
            options.Validate();
        }
    }
}