using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;

using WowsGlobal.Models;

namespace WowCMS.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public PhoneController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        #region Get Phone By EmployeeID
        /// <summary>
        /// Get Phone By EmployeeID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/umbraco/api/GetPhoneByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetPhoneByEmployeeId(int employeeId)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(employeeId).Children().Where(x => x.ContentType.Alias.Equals("phone")).FirstOrDefault();
                if (query != null)
                {
                    if (query.ContentType.Alias != "phone")
                    {
                        return NotFound();
                    }
                    Phone result = new Phone()
                    {
                        Id = query.Id,
                        CountryId = query.HasValue("phoneCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("phoneCountry")).Id : 0,
                        PhoneNumber = query.Value<string>("phoneNumber")

                    };
                   
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region Get Phone By ShareHolderId
        /// <summary>
        /// Get Phone By shareholderId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/umbraco/api/GetPhoneByShareHolderId/{shareholderId}")]
        public async Task<IActionResult> GetPhoneByShareHolderId(int shareholderId)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(shareholderId).Children().Where(x => x.ContentType.Alias.Equals("phone")).FirstOrDefault();
                if (query != null)
                {
                    if (query.ContentType.Alias != "phone")
                    {
                        return NotFound();
                    }
                    Phone result = new Phone()
                    {
                        Id = query.Id,
                        CountryId = query.HasValue("phoneCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("phoneCountry")).Id : 0,
                        PhoneNumber = query.Value<string>("phoneNumber")

                    };
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region GetPhone
        /// <summary>
        /// Get the Phone by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhone(int id)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(id);
                if (query != null)
                {
                    if (query.ContentType.Alias != "phone")
                    {
                        return NotFound();
                    }
                    Phone result = new Phone()
                    {
                        Id = query.Id,
                        CountryId = query.HasValue("phoneCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("phoneCountry")).Id : 0,
                        PhoneNumber = query.Value<string>("phoneNumber")

                    };
                    return Ok(result);
                   
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region Add Phone

        /// <summary>
        /// Add Phone in the Umbraco Backoffice
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPhone([FromBody] Phone phone)
        {
            try
            {
                int id = 0;
                IPublishedContent query = _contentQuery.Content(phone.ShareholderId);
                IPublishedContent employee = _contentQuery.Content(phone.employeeId);

                if (phone.ShareholderId != 0 && query !=null)                
                    id= query.Id;                
                else if(phone.employeeId !=0 && employee != null)
                    id = employee.Id;


                //Add Phone in ShareHolder
                if (query != null || employee !=null)
                {
                    var objphone = _contentService.Create("phone Number", id, "phone");
                    objphone.CreateDate = DateTime.UtcNow;
                    objphone.UpdateDate = DateTime.UtcNow;
                    objphone.SetValue("phoneNumber", phone.PhoneNumber);
                    if (phone.CountryId != 0)
                    {
                        var content = _contentService.GetById((int)phone.CountryId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                        objphone.SetValue("phoneCountry", udi.ToString());
                    }
                    _contentService.SaveAndPublish(objphone);
                    phone.Id = objphone.Id;

                    return Ok(phone);
                }
           
           
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update Phone
        /// <summary>
        /// Update the Phone
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePhone([FromBody] Phone phone)
        {
            try
            {


                IPublishedContent query = _contentQuery.Content(phone.Id);
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    if (toUpdate != null)
                    {
                        toUpdate.UpdateDate = DateTime.UtcNow;
                        toUpdate.SetValue("phoneNumber", phone.PhoneNumber);
                        if (phone.CountryId != 0)
                        {
                            var content = _contentService.GetById((int)phone.CountryId);
                            // Create an Udi of the page
                            var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                            toUpdate.SetValue("phoneCountry", udi.ToString());
                        }
                        _contentService.SaveAndPublish(toUpdate);
                    }

                    return Ok(toUpdate);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Delete Phone
        /// <summary>
        /// Delete phone by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete]
        public async Task<IActionResult> DeletePhone(int id)
        {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "phone")
                {
                    _contentService.Delete(toDelete);
                    return Ok("Successfully Deleted");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
