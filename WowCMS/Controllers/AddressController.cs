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
    [Route("umbraco/api/[controller]")]
    [ApiController]
    public class AddressController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public AddressController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        #region Get Address By EmployeeID
        /// <summary>
        /// Get Address By EmployeeID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/umbraco/api/GetAddressByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetAddressByEmployeeId(int employeeId)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(employeeId).Children().Where(x => x.ContentType.Alias.Equals("address")).FirstOrDefault();
                if (query != null)
                {
                    if (query.ContentType.Alias != "address")
                    {
                        return NotFound();
                    }
                    Address result = new Address
                    {
                        Id = query.Id,
                        City = query.Value<string>("city"),
                        Address1 = query.Value<string>("Address1"),
                        Address2 = query.Value<string>("Address2"),
                        PostalCode = query.Value<string>("PostalCode"),
                        District = query.Value<string>("District"),
                        CountryId = query.HasValue("addressCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("addressCountry")).Id : 0

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

        #region Get Address By ShareHolderId
        /// <summary>
        /// Get Address By shareholderId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/umbraco/api/GetAddressByShareHolderId/{shareholderId}")]
        public async Task<IActionResult> GetAddressByShareHolderId(int shareholderId)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(shareholderId).Children().Where(x=>x.ContentType.Alias.Equals("address")).FirstOrDefault();
                if (query != null)
                {
                    if (query.ContentType.Alias != "address")
                    {
                        return NotFound();
                    }
                    Address result = new Address
                    {
                        Id = query.Id,
                        City = query.Value<string>("city"),
                        Address1 = query.Value<string>("Address1"),
                        Address2 = query.Value<string>("Address2"),
                        PostalCode = query.Value<string>("PostalCode"),
                        District = query.Value<string>("District"),
                        CountryId = query.HasValue("addressCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("addressCountry")).Id : 0

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

        #region GetAddress
        /// <summary>
        /// Get the Address by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id) {
            try
            {
                IPublishedContent query = _contentQuery.Content(id);
                if (query != null)
                {
                    if (query.ContentType.Alias != "address")
                    {
                        return NotFound();
                    }
                    Address result = new Address
                    {
                        Id = query.Id,
                        City = query.Value<string>("city"),
                        Address1 = query.Value<string>("Address1"),
                        Address2 = query.Value<string>("Address2"),
                        PostalCode = query.Value<string>("PostalCode"),
                        District = query.Value<string>("District"),
                        CountryId = query.HasValue("addressCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)query.Value("addressCountry")).Id : 0

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

        #region Add Addresses

        /// <summary>
        /// Add Address in the Umbraco Backoffice
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] Address address) {
            try
            {
                int id = 0;
                IPublishedContent queryShareholder = _contentQuery.Content(address.ShareholderId);
                IPublishedContent employee = _contentQuery.Content(address.EmployeeId);

                if(queryShareholder != null && address.ShareholderId !=0)
                id= queryShareholder.Id;
                else if(employee != null && address.EmployeeId !=0)
                id = employee.Id;
                

                if (queryShareholder != null || employee !=null)
                {
                    var addaddress = _contentService.Create("Home Address", id, "address");
                    addaddress.CreateDate = DateTime.UtcNow;
                    addaddress.UpdateDate = DateTime.UtcNow;
                    addaddress.SetValue("city", address.City);
                    addaddress.SetValue("address1", address.Address1);
                    addaddress.SetValue("address2", address.Address2);
                    addaddress.SetValue("PostalCode", address.PostalCode);
                    addaddress.SetValue("District", address.District);
                    // Get the page using the GUID you've defined

                    if (address.CountryId != 0)
                    {
                        var content = _contentService.GetById((int)address.CountryId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                        addaddress.SetValue("addressCountry", udi.ToString());
                    }
                    _contentService.SaveAndPublish(addaddress);
                    address.Id = addaddress.Id;
                  
                    return Ok(address);

                }

            
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update Address
        /// <summary>
        /// Update the Address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] Address address)
        {
            try
            {


                IPublishedContent query = _contentQuery.Content(address.Id);
                   
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    if(toUpdate != null)
                    {
                        toUpdate.SetValue("city", address.City);
                        toUpdate.SetValue("address1", address.Address1);
                        toUpdate.SetValue("address2", address.Address2);
                        toUpdate.SetValue("PostalCode", address.PostalCode);
                        toUpdate.SetValue("District", address.District);
                        // Get the page using the GUID you've defined

                        if (address.CountryId != 0)
                        {
                            var content = _contentService.GetById((int)address.CountryId);
                            // Create an Udi of the page
                            var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                            toUpdate.SetValue("addressCountry", udi.ToString());
                        }
                        
                        toUpdate.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(toUpdate);
                        address.Id = toUpdate.Id;
                    }
                
                    return Ok(address);
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

        #region Delete Address
        /// <summary>
        /// Delete Address by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int id) {
            try
            {
                
                    var toDelete = _contentService.GetById(id);
                    if (toDelete.ContentType.Alias == "address")
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
