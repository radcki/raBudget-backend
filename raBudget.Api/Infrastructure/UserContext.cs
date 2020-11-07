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
        private readonly IHttpContextAccessor _httpContext;

        public UserContext(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string UserId => _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
