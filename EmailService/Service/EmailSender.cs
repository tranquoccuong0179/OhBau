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
    public class EmailSender(EmailSetting _emailSetting)
    {
        public async Task SendMailSMTP(string toEmail, string subject, string message)
        {
            var emailSender = CreateEmailMessage(toEmail, subject, message);
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
                await client.ConnectAsync(_emailSetting.SmtpServer, _emailSetting.Port, _emailSetting.UseSsl);
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

