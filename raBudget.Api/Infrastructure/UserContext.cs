using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using raBudget.Domain.Interfaces;

namespace raBudget.Api.Infrastructure
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContext)
        {
            UserId = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string UserId { get; private set; }
        
        public void SetFromAuthenticationResult(ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            catch (Exception e)
            {

            }
        }
    }
}
