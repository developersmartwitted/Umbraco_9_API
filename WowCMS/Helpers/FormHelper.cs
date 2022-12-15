using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WowsGlobal.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace WowCMS.Helpers {
    public static class FormHelper {
        public static bool IsValidEmail(string emailaddress) {
            try {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            } catch (FormatException) {
                return false;
            }
        }
        public static bool IsCleanString(string validateString, bool isRequired) {
            char[] invalidChars = { '<', '>', '&', '%', ';', '=', '{', '}' };
            try {
                if (isRequired && string.IsNullOrEmpty(validateString)) return false;
                if (!string.IsNullOrEmpty(validateString)) {
                    foreach (char c in invalidChars) {
                        if (validateString.Contains(c)) {
                            return false;
                        }
                    }
                }
                return true;
            } catch (FormatException) {
                return false;
            }
        }
        public static bool IsCaptchaValid(string secretKey, string response) {
            return (ReCaptchaClass.Validate(secretKey, response) == "true" ? true : false);
        }
        public static void SendMail(IConfiguration config, string fromEmail, string toEmail, string subject, string body) {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient()) {
                client.Connect(config["Smtp:Endpoint"], int.Parse(config["Smtp:Port"]), true);
                client.Authenticate(config["Smtp:User"], config["Smtp:Password"]);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
