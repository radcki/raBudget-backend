using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using raBudget.Domain.Models;

namespace raBudget.Application.Features.Account.Command
{
    public class RegisterUser
    {
        public class Command : IRequest<Result>
        {
            public string SubjectId { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class Result
        {
            public string UserId { get; set; }
        }

        public class Success : INotification
        {
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IUserStore<ApplicationUser> _userStore;
            private readonly IUserEmailStore<ApplicationUser> _emailStore;
            private readonly IMediator _mediator;
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Password != request.ConfirmPassword)
                {
                    //todo
                }
                var user = new ApplicationUser();
                await _userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, request.Password);
                
                if (result.Succeeded)
                {

                    var userId = await _userManager.GetUserIdAsync(user);

                    //var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //await _emailSender.SendEmailAsync(email, "Confirm your email",
                    //                                  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    _ = _mediator.Publish(new Success()
                                          {
                                              UserId = userId
                                          }, cancellationToken);

                    return new Result()
                           {
                               UserId = userId
                           };
                }
                else
                {
                    var errors = result.Errors;
                    throw new Exception();
                }
            }

        }
    }
}
