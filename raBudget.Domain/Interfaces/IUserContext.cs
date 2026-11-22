using System.Security.Claims;

namespace raBudget.Domain.Interfaces
{
    public interface IUserContext
    {
        string UserId { get; }
        void SetFromAuthenticationResult(ClaimsPrincipal claimsPrincipal);
    }
}
