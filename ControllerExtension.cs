using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using bimeh_back.Models;
using bimeh_back.Components;
using bimeh_back.Components.Configs;
using bimeh_back.Components.Extensions;
using FCM.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using ContentType = System.Net.Mime.ContentType;

namespace bimeh_back.Components.Extensions
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v1")]
    public class ControllerExtension : Controller, IPaginationExtension, IResponseExtension, IUploadExtension
    {
        protected ComponentConfig ComponentConfig;
        public AppDbContext Context;
        private readonly MailSettings _mailSettings;
        public IWebHostEnvironment Environment { get; }
        public object AuthenticatedUser { get; set; }
        public List<object> DataBag { get; set; } = new List<object>();
        protected IServiceScopeFactory ServiceScopeFactory { get; }

        public ControllerExtension(AppDbContext context, IWebHostEnvironment environment,
            IOptions<ComponentConfig> config, IServiceScopeFactory serviceScopeFactory)
        {
            ComponentConfig = config.Value;
            Context = context;
            Environment = environment;
            ServiceScopeFactory = serviceScopeFactory;
        }


        protected string GenerateCode(int length = 5, bool useLetter = false)
        {
            var random = new Random((int) DateTime.Now.ToFileTime());
            string data;
            if (useLetter)
            {
                data = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }
            else
            {
                data = "0123456789";
            }

            var chars = Enumerable.Range(0, length)
                .Select(x => data[random.Next(0, data.Length)]);
            return new string(chars.ToArray());
        }

        protected JsonResult Ok(object data = null, string msg = null)
        {
            return (this as IResponseExtension).Ok(data, msg);
        }

        protected JsonResult OkMsg(string msg = null)
        {
            return (this as IResponseExtension).OkMsg(msg);
        }

        protected JsonResult NotFound(object data = null, string msg = "داده پیدا نشد.")
        {
            return (this as IResponseExtension).NotFound(data, msg);
        }

        protected JsonResult NotFoundMsg(string msg = "داده پیدا نشد.")
        {
            return (this as IResponseExtension).NotFoundMsg(msg);
        }

        protected JsonResult PermissionDenied(object data = null, string msg = null)
        {
            return (this as IResponseExtension).PermissionDenied(data, msg);
        }

        protected JsonResult PermissionDeniedMsg(string msg = null)
        {
            return (this as IResponseExtension).PermissionDeniedMsg(msg);
        }

        protected JsonResult NotAuth(object data = null, string msg = "لطفا وارد سیستم شوید.")
        {
            return (this as IResponseExtension).NotAuth(data, msg);
        }

        protected JsonResult NotAuthMsg(string msg = "لطفا وارد سیستم شوید.")
        {
            return (this as IResponseExtension).NotAuthMsg(msg);
        }

        protected JsonResult BadRequest(object data = null, string msg = null)
        {
            return (this as IResponseExtension).BadRequest(data, msg);
        }

        protected JsonResult BadRequestMsg(string msg = null)
        {
            return (this as IResponseExtension).BadRequestMsg(msg);
        }

        protected JsonResult InternalError(object data = null, string msg = null)
        {
            return (this as IResponseExtension).InternalError(data, msg);
        }

        protected JsonResult InternalErrorMsg(string msg = null)
        {
            return (this as IResponseExtension).InternalErrorMsg(msg);
        }

        protected string GenerateJWTToken(IDictionary<string, string> claimDictionary, DateTime expireDate)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ComponentConfig.Jwt.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = claimDictionary
                .Select(x => new Claim(x.Key, x.Value))
                .ToArray();
            // var claims = new[] {
            //     new Claim("id", model.Id.ToString()),
            //     // new Claim("phone", user.Phone),
            //     // new Claim("role", user.IsCompleted ? Policies.User : Policies.IncompleteUser),
            //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            // };
            var token = new JwtSecurityToken(
                issuer: ComponentConfig.Jwt.Issuer,
                audience: ComponentConfig.Jwt.Audience,
                claims: claims,
                expires: expireDate,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected async Task SendNotification(List<string> tokens, string notificationTitle = null,
            string notificationBody = null)
        {
            using var sender = new Sender(ComponentConfig.Fcm.ServerKey);
            var notification = new FCM.Net.Message
            {
                RegistrationIds = tokens,
                Notification = new FCM.Net.Notification
                {
                    Title = notificationTitle,
                    Body = notificationBody,
                    Icon = "https://atiito.com/Uploads/Logo/logo.png",
                    ClickAction = "https://atiito.com/"
                },
                Data = null,
            };
            await sender.SendAsync(notification);
        }

        // [ApiExplorerSettings(IgnoreApi = true)]
        //
        // public async Task SendEmailAsync(string ToEmail , string Subject , string Body ,List<IFormFile> Attachments )
        // {
        //     var email = new MimeMessage();
        //     email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        //     email.To.Add(MailboxAddress.Parse(ToEmail));
        //     email.Subject =Subject;
        //     var builder = new BodyBuilder();
        //     if (Attachments != null)
        //     {
        //         byte[] fileBytes;
        //         foreach (var file in Attachments)
        //         {
        //             if (file.Length > 0)
        //             {
        //                 using (var ms = new MemoryStream())
        //                 {
        //                     file.CopyTo(ms);
        //                     fileBytes = ms.ToArray();
        //                 }
        //
        //                 builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
        //             }
        //         }
        //     }
        //
        //     builder.HtmlBody = Body;
        //     email.Body = builder.ToMessageBody();
        //     using var smtp = new SmtpClient();
        //     smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        //     smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        //     await smtp.SendAsync(email);
        //     smtp.Disconnect(true);
        // }
    }
}