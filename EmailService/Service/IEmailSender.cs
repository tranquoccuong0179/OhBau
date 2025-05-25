using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Request;

namespace EmailService.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailDTO request);
        Task SendEmailWithAttachmentAsync(string email, string subject, string message, string attachmentPath);
    }
}
