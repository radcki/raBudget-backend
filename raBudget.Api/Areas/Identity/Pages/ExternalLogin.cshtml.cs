using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using raBudget.Domain.Models;

namespace raBudget.Api.Areas.Identity.Pages
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly TestUserStore _users;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender, 
            IEventService events, 
            IIdentityServerInteractionService interaction, 
            TestUserStore users = null)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _events = events;
            _users = users;
            _interaction = interaction;
            _users = users ?? new TestUserStore(new List<TestUser>());
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", "Callback", new { returnUrl });

            var props = new AuthenticationProperties
                        {
                            RedirectUri = redirectUrl,
                            Items =
                            {
                                { "returnUrl", returnUrl },
                                { "scheme", provider },
                            }
                        };

            return Challenge(props, provider);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string remoteError = null)
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            var (user, provider, providerUserId, displayName, claims) = FindUserFromExternalProvider(result);
            if (user == null)
            {
                user = await AutoProvisionUserAsync(provider, providerUserId, claims);
            }
            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, displayName));

            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties()
                                   {
                                       ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15),
                                       IsPersistent = true,
                                       AllowRefresh = true,
                                       IssuedUtc = DateTimeOffset.UtcNow,
                                       RedirectUri = returnUrl,
                                       Parameters = { },
                                       Items = { }
                                   };
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            await _signInManager.SignInAsync(user, localSignInProps);
            await _signInManager.ExternalLoginSignInAsync(provider,providerUserId, isPersistent: false, bypassTwoFactor: true);

            
            var principal = await _signInManager.ClaimsFactory.CreateAsync(user);
            await HttpContext.SignInAsync(principal, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            
            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));
           
            return Redirect(returnUrl);
        }

        //public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        //{
        //    returnUrl = returnUrl ?? Url.Content("~/");
        //    // Get the information about the user from the external login provider
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        ErrorMessage = "Error loading external login information during confirmation.";
        //        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };

        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

        //                var userId = await _userManager.GetUserIdAsync(user);
        //                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //                var callbackUrl = Url.Page(
        //                    "/Account/ConfirmEmail",
        //                    pageHandler: null,
        //                    values: new { area = "Identity", userId = userId, code = code },
        //                    protocol: Request.Scheme);

        //                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
        //                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //                // If account confirmation is required, we need to show the link if we don't have a real email sender
        //                if (_userManager.Options.SignIn.RequireConfirmedAccount)
        //                {
        //                    return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
        //                }

        //                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

        //                return LocalRedirect(returnUrl);
        //            }
        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    ProviderDisplayName = info.ProviderDisplayName;
        //    ReturnUrl = returnUrl;
        //    return Page();
        //}
        private (ApplicationUser user, string provider, string providerUserId, string displayName, IEnumerable<Claim> claims) FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");
            var email = externalUser.FindFirst(ClaimTypes.Email).Value;
            var claims = externalUser.Claims.ToList();
            
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            var name = externalUser.FindFirst(ClaimTypes.Name).Value 
                       ?? CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

            //var user = _users.FindByExternalProvider(provider, providerUserId);
            var user =  _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
            return (user, provider, providerUserId, name, claims);
        }
        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                else
                {
                    filtered.Add(claim);
                }
            }

            if (filtered.All(x => x.Type != JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id

            // check if a display name is available, otherwise fallback to subject id
            var email = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value;

            // create new user
            var user = new ApplicationUser()
                       {
                           Email = email,
                           UserName = email
                       };
            var result = await _userManager.CreateAsync(user);
            return user;
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}
