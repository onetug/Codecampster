using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);
    }
}
