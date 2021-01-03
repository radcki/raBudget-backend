using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace raBudget.Domain.Interfaces
{
    public interface IUserContext
    {
        string UserId { get; }
        void SetFromAuthenticationResult(ClaimsPrincipal claimsPrincipal);
    }
}
