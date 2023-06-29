using System.Collections.Generic;
using System.Threading.Tasks;
using bimeh_back.Models;
using Microsoft.AspNetCore.Http;

namespace bimeh_back.Components.Services.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(string ToEmail, string Subject, string Body, IFormFile Attachments);
    }
}