using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using raBudget.Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace raBudget.Infrastructure.Services
{
    public class EmailClient : IEmailClient
    {
        #region Implementation of IEmailClient

        /// <inheritdoc />
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress("no-reply@rabt.pl", "raBudget"));

            var recipients = new List<EmailAddress>
                             {
                                 new EmailAddress(email, email),
                             };
            msg.AddTos(recipients);

            msg.SetSubject(subject);

            //msg.AddContent(MimeType.Text, "Hello World plain text!");
            msg.AddContent(MimeType.Html, htmlMessage);

            var client = new SendGridClient("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");
            var response = await client.SendEmailAsync(msg);
        }

        #endregion
    }
}
