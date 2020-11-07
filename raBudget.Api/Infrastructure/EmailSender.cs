using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using raBudget.Domain.Interfaces;

namespace raBudget.Api.Infrastructure
{
    public class EmailSender: IEmailSender
    {
        private readonly IEmailClient _emailClient;

        public EmailSender(IEmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _emailClient.SendEmailAsync(email, subject, htmlMessage);
        }

    }
}
