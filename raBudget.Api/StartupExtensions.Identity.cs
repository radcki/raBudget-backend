using System.Threading.Tasks;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using raBudget.Api.Infrastructure;
using raBudget.Domain.Interfaces;

namespace raBudget.Api
{
    public static partial class StartupExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    .AddJwtBearer(options => ConfigureJwtBearer(options, configuration))
                    .AddOAuth2Introspection("introspection", options =>
                                                             {
                                                                 options.Authority = configuration["Authentication:Authority"];
                                                                 options.ClientId = configuration["Authentication:Audience"];
                                                             });

            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
        }

        public static void UseIdentityServices(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void ConfigureJwtBearer(JwtBearerOptions options, IConfiguration configuration)
        {
            options.Authority = configuration["Authentication:Authority"];
            options.Audience = configuration["Authentication:Audience"];

            options.TokenValidationParameters = new TokenValidationParameters()
                                                {
                                                    ValidateLifetime = true,
                                                    ValidateIssuer = true,
                                                    ValidateAudience = true,
                                                    ValidateIssuerSigningKey = true,
                                                    ValidTypes = new[] { "at+jwt", "JWT" },
                                                    ValidIssuers = [configuration["Authentication:Authority"]],
                                                    ValidAudience = configuration["Authentication:Audience"],
                                                };

            options.RequireHttpsMetadata = false;
            options.Events = new JwtBearerEvents
                             {
                                 OnTokenValidated = async c =>
                                                    {
                                                        await Task.Run(() =>
                                                                       {
                                                                           // Update user context with authentication result - required for signalr
                                                                           var userContext = c.HttpContext
                                                                                              .RequestServices
                                                                                              .GetRequiredService<IUserContext>();

                                                                           userContext.SetFromAuthenticationResult(c.Principal);
                                                                       });
                                                    },
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

            options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
        }
    }
}