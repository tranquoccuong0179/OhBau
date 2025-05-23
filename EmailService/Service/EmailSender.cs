using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EmailService.Config;
using MimeKit;
using MailKit.Net.Smtp;

namespace EmailService.Service
{
    public class EmailSender(EmailSetting _emailSetting) : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = CreateEmailMessage(email, subject, message);
            await SendAsync(emailMessage);
        }

        public Task SendEmailWithAttachmentAsync(string email, string subject, string message, string attachmentPath)
        {
            throw new NotImplementedException();
        }

        private MimeMessage CreateEmailMessage(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSetting.FromName, _emailSetting.From));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                var options = _emailSetting.Port switch
                {
                    465 => MailKit.Security.SecureSocketOptions.SslOnConnect,
                    587 => MailKit.Security.SecureSocketOptions.StartTls,
                    _ => _emailSetting.UseSsl ? MailKit.Security.SecureSocketOptions.Auto : MailKit.Security.SecureSocketOptions.None
                };

                await client.ConnectAsync(_emailSetting.SmtpServer, _emailSetting.Port, options);
                await client.AuthenticateAsync(_emailSetting.UserName, _emailSetting.Password);
                await client.SendAsync(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}

