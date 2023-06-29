using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using bimeh_back.Components.Configs;
using bimeh_back.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;

namespace bimeh_back.Components.Services.MailService
{
    public class MailService : IMailService
    {
        private static MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public static async Task SendEmailAsync(string ToEmail, string Subject, string Body, IFormFile Attachments)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(ToEmail));
            email.Subject = Subject;
            var builder = new BodyBuilder();
            if (Attachments != null)
            {
                byte[] fileBytes;

                var file = Attachments;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            }

            builder.HtmlBody = Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        Task IMailService.SendEmailAsync(string ToEmail, string Subject, string Body, IFormFile Attachments)
        {
            return SendEmailAsync(ToEmail, Subject, Body, Attachments);
        }
    }
}