using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Services
{
    public class AuthMessageEmailSender : IEmailSender
    {
        public AuthMessageEmailSender(IOptions<AppOptions> options)
        {
            Options = options.Value;
        }

        public AppOptions Options { get; }

        public Task SendEmailAsync(string emailAddress, string subject, string message)
        {
            return Execute(subject, message, emailAddress);
        }

        public Task Execute(string subject, string message,
            string emailAddress)
        {
            var _message = new MimeMessage();
            _message.Subject = subject;
            _message.From.Add(new MailboxAddress("Orlando Code Camp", Options.Account));
            _message.To.Add(new MailboxAddress(emailAddress, emailAddress));
            _message.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                client.Connect(Options.EmailServer, Options.PortNumber, false);

                // We don't have an OAuth2 token, so we've disabled it
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(Options.Account, Options.Password);

                client.Send(_message);
                client.Disconnect(true);
            }

            return Task.FromResult(0);
        }
    }
}
