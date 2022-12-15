using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;
using WowCMS.Helpers;
using WowsGlobal.Models;

namespace WowCMS.Controllers {
    [ApiController]
    [Route("umbraco/api/[controller]")]
    public class FormController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IConfiguration _configuration;

        public FormController(IPublishedContentQuery contentQuery, IContentService contentService, IConfiguration configuration) {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _configuration = configuration;
        }
        [HttpPost("NewsletterSubscribe")]
        public async Task<IActionResult> NewsLetterSubscribe([FromBody] FormNewsletterSubscribe subscriber) {
            try {
                // validate fields
                if (!FormHelper.IsValidEmail(subscriber.Email) || 
                    !FormHelper.IsCleanString(subscriber.Email, true)) return BadRequest("Invalid Email Address given");

                // check for preexisting
                subscriber.Email = subscriber.Email.Trim();
                string searchFor = subscriber.Email.ToUpper();
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//newsletterSubscribers");
                IPublishedContent query = contentRoot.Children()
                    .Where(x => x.Value<string>("emailAddress").ToUpper() == searchFor)
                    .FirstOrDefault();
                if (query == null) {
                    // add to cms
                    var request = _contentService.Create(subscriber.Email, contentRoot.Id, "newsletterSubscriber");
                    request.SetValue("emailAddress", subscriber.Email);
                    request.CreateDate = DateTime.UtcNow;
                    request.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(request);
                    subscriber.Id = request.Id;

                    // notify by email
                    var newsletterEmail = _configuration["Contact:Newsletter"];
                    var fromEmail = _configuration["Contact:AdminEmail"];
                    string subject = "Newsletter Subscribe";
                    string body = string.Format("<b>{0}</b><br />" +
                            "Email: {1}", subject, subscriber.Email);
                    FormHelper.SendMail(_configuration, newsletterEmail, newsletterEmail, subject, body);

                    return Ok(subscriber);
                } else {
                    return BadRequest("Email Address already subscribed");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ContactRequest")]
        public async Task<IActionResult> ContactRequest([FromBody] FormContactRequest contact) {
            try {
                // validate fields
                var secretKey = _configuration["Recaptcha:SecretKey"];
                if (!FormHelper.IsCaptchaValid(secretKey, contact.Recaptcha)) return BadRequest("Recaptcha not validated");
                if (!FormHelper.IsValidEmail(contact.Email) || 
                    !FormHelper.IsCleanString(contact.Email, true)) return BadRequest("Invalid Email Address given");
                if (!FormHelper.IsCleanString(contact.FirstName, true)) return BadRequest("Name contains invalid characters or symbols");
                if (!FormHelper.IsCleanString(contact.LastName, true)) return BadRequest("Name contains invalid characters or symbols");
                if (!FormHelper.IsCleanString(contact.Topic, false)) return BadRequest("Topic contains invalid characters or symbols");
                if (!FormHelper.IsCleanString(contact.Note, false)) return BadRequest("Note contains invalid characters or symbols");

                // add to cms
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//contactRequests");
                var requestTitle = contact.FirstName + " " + contact.LastName + ":" + contact.Email;
                var request = _contentService.Create(requestTitle, contentRoot.Id, "contactRequest");
                request.SetValue("firstName", contact.FirstName);
                request.SetValue("lastName", contact.LastName);
                request.SetValue("emailAddress", contact.Email);
                request.SetValue("topic", contact.Topic);
                request.SetValue("note", contact.Note);
                request.CreateDate = DateTime.UtcNow;
                request.UpdateDate = DateTime.UtcNow;
                _contentService.SaveAndPublish(request);
                contact.Id = request.Id;

                // notify by email
                var contactUsEmail = _configuration["Contact:ContactUs"];
                var fromEmail = _configuration["Contact:AdminEmail"];
                string subject = "Contact request";
                string body = string.Format("<b>{0}</b><br />" +
                    "First Name: {1}<br />" +
                    "Last Name: {2}<br />" +
                    "Email: {3}<br />" +
                    "Topic: {4}<br />" +
                    "Note: {5}", subject, contact.FirstName, contact.LastName, contact.Email, contact.Topic, contact.Note);
                FormHelper.SendMail(_configuration, contactUsEmail, contactUsEmail, subject, body);

                return Ok(contact);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
