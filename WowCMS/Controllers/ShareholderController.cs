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
    public class ShareholderController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public ShareholderController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        #region Get ShareHolders By CompanyId
        /// <summary>
        /// Get Shareholders through Umbraco Backoffice
        /// </summary>
        /// <returns></returns>
         [HttpGet("GetShareholdersByCompanyId/{companyId}")]
        public async Task<IActionResult> GetShareholdersByCompanyId(int companyId) {
            try
            {

                var GetCompany = _contentQuery.Content(companyId);
                if(GetCompany != null)
                {
                    IPublishedContent query = GetCompany.Children().Where(x=>x.ContentType.Alias.Equals("shareholders")).FirstOrDefault();
                    if (query !=null)
                    {
                        List<Shareholder> results = new List<Shareholder>();
                        List<Address> addresses = new List<Address>();
                        List<Phone> phones = new List<Phone>();
                        foreach (var item in query.Children().OrderBy(x=>x.SortOrder))
                        {
                            var ChildItems = item.Children();
                            if (ChildItems.Count() > 0)
                            {
                                var address = ChildItems.Where(x => x.ContentType.Alias.Equals("address")).FirstOrDefault();
                                var phone = ChildItems.Where(x => x.ContentType.Alias.Equals("phone")).FirstOrDefault();
                                Address result = new Address
                                {
                                    Id = address.Id,
                                    City = address.Value<string>("city"),
                                    Address1 = address.Value<string>("Address1"),
                                    Address2 = address.Value<string>("Address2"),
                                    PostalCode = address.Value<string>("PostalCode"),
                                    District = address.Value<string>("District"),
                                    CountryId = address.HasValue("addressCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)address.Value("addressCountry")).Id : 0,
                                    ShareholderId = item.Id

                                };
                                if (address != null)
                                {

                                    addresses.Add(result);
                                }
                                if (phone != null)
                                {

                                    Phone _objphone = new Phone()
                                    {
                                        Id = phone.Id,
                                        CountryId = phone.HasValue("phoneCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)phone.Value("phoneCountry")).Id : 0,
                                        PhoneNumber = phone.Value<string>("phoneNumber")

                                    };
                                    phones.Add(_objphone);
                                }


                            }

                            results.Add(new Shareholder
                            {
                                Id = item.Id,
                                CompanyId = item.Parent.Parent.Parent.Id,
                                BrandName = item.Value<string>("brandName"),
                                FirstName = item.Value<string>("firstName"),
                                LastName = item.Value<string>("lastName"),
                                MiddleName = item.Value<string>("middleName"),
                                Email = item.Value<string>("email"),
                                InvestorType = item.Value<int>("investorType"),
                                InstituitionalType = item.Value<int>("institutionalType"),
                                Status = item.Value<int>("status"),
                                ShareholderType = item.Value<int>("shareholderType"),
                                TaxCountryId = item.HasValue("taxCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)item.Value("taxCountry")).Id : 0,
                                CitizenshipCountryId = item.HasValue("citizenshipCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)item.Value("citizenshipCountry")).Id : 0,
                                Addresses = addresses,
                                Phones = phones
                            });
                        }
                        return Ok(results);
                    }
                    return NotFound();

                }else
                    return NotFound("No Company found");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        #region Get ShareHolder By Id
        /// <summary>
        /// Get SHareholder By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShareholder(int id) {
            try
            {
              var shareholderquery = _contentQuery.Content(id);
                if (shareholderquery !=null && shareholderquery.ContentType.Alias=="shareholder")
                {
                   
                    List<Address> addresses = new List<Address>();
                    List<Phone> phones = new List<Phone>();
                  
                        var ChildItems = shareholderquery.Children();
                        if (ChildItems.Count() > 0)
                        {
                            var address = ChildItems.Where(x => x.ContentType.Alias.Equals("address")).FirstOrDefault();
                            var phone = ChildItems.Where(x => x.ContentType.Alias.Equals("phone")).FirstOrDefault();
                            Address result = new Address
                            {
                                Id = address.Id,
                                City = address.Value<string>("city"),
                                Address1 = address.Value<string>("Address1"),
                                Address2 = address.Value<string>("Address2"),
                                PostalCode = address.Value<string>("PostalCode"),
                                District = address.Value<string>("District"),
                                CountryId = address.HasValue("addressCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)address.Value("addressCountry")).Id : 0,
                                ShareholderId = shareholderquery.Id

                            };
                            if (address != null)
                            {

                                addresses.Add(result);
                            }
                            if (phone != null)
                            {

                                Phone _objphone = new Phone()
                                {
                                    Id = phone.Id,
                                    CountryId = phone.HasValue("phoneCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)phone.Value("phoneCountry")).Id : 0,
                                    PhoneNumber = phone.Value<string>("phoneNumber")

                                };
                                phones.Add(_objphone);
                            }


                        }
                    Shareholder results = new Shareholder()
                    {
                        Id = shareholderquery.Id,
                        CompanyId = shareholderquery.Parent.Parent.Parent.Id,
                        BrandName = shareholderquery.Value<string>("brandName"),
                        FirstName = shareholderquery.Value<string>("firstName"),
                        LastName = shareholderquery.Value<string>("lastName"),
                        MiddleName = shareholderquery.Value<string>("middleName"),
                        Email = shareholderquery.Value<string>("email"),
                        InvestorType = shareholderquery.Value<int>("investorType"),
                        InstituitionalType = shareholderquery.Value<int>("institutionalType"),
                        Status = shareholderquery.Value<int>("status"),
                        ShareholderType = shareholderquery.Value<int>("shareholderType"),
                        TaxCountryId = shareholderquery.HasValue("taxCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)shareholderquery.Value("taxCountry")).Id : 0,
                        CitizenshipCountryId = shareholderquery.HasValue("citizenshipCountry") ? ((Umbraco.Cms.Core.Models.PublishedContent.PublishedContentWrapped)shareholderquery.Value("citizenshipCountry")).Id : 0,
                        Addresses = addresses,
                        Phones = phones
                    };
                  
                    
                    return Ok(results);
                }
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Add Shareholder By Company Id
        /// <summary>
        /// Add the shareholder By CompanyId
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddShareholder([FromBody] Shareholder shareholder) {

            try
            {
                var company = _contentQuery.Content(shareholder.CompanyId);
                if (company != null)
                {

                    int sharholdersFolderId = 0;
                    IPublishedContent query = null;
                    IPublishedContent contentRoot =company.Children().Where(x=>x.ContentType.Alias.Equals("shareholders")).FirstOrDefault();
                    if (contentRoot == null)
                    {
                        // add ShareHolder folder
                        var shareholderFolder = _contentService.Create("Shareholders", company.Id, "shareholders");
                        shareholderFolder.CreateDate = DateTime.UtcNow;
                        shareholderFolder.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(shareholderFolder);
                        sharholdersFolderId = shareholderFolder.Id;
                    }
                    else
                    {
                         query = contentRoot.Children()
                        .Where(x => x.Value<string>("brandName").ToLower() == shareholder.BrandName.ToLower())
                        .FirstOrDefault();

                        sharholdersFolderId= contentRoot.Id;
                    }
                    
                    if (query == null)
                    {
                        var request = _contentService.Create(shareholder.BrandName,sharholdersFolderId, "shareholder");
                        request.SetValue("brandName", shareholder.BrandName);
                        request.SetValue("firstName", shareholder.FirstName);
                        request.SetValue("lastName", shareholder.LastName);
                        request.SetValue("middleName", shareholder.MiddleName);
                        request.SetValue("legalName", shareholder.LegalName);
                        request.SetValue("email", shareholder.Email);
                        request.SetValue("shareholderType", shareholder.ShareholderType);
                        request.SetValue("institutionalType", shareholder.InstituitionalType);
                        request.SetValue("investorType", shareholder.InvestorType);
                        request.SetValue("status", shareholder.Status);

                        if (shareholder.CitizenshipCountryId != 0)
                        {
                            var content = _contentService.GetById((int)shareholder.CitizenshipCountryId);
                            // Create an Udi of the page
                            var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                            request.SetValue("citizenshipCountry", udi.ToString());
                        }
                        if (shareholder.TaxCountryId != 0)
                        {
                            var content = _contentService.GetById((int)shareholder.TaxCountryId);
                            // Create an Udi of the page
                            var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                            request.SetValue("taxCountry", udi.ToString());
                        }
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(request);
                        shareholder.Id = request.Id;



                        // Add address for this shareholder
                        if (shareholder.Addresses.Count() > 0)
                        {
                            foreach (var item in shareholder.Addresses)
                            {
                                var address = _contentService.Create("Home Address", request.Id, "address");
                                address.CreateDate = DateTime.UtcNow;
                                address.UpdateDate = DateTime.UtcNow;
                                address.SetValue("city", item.City);
                                address.SetValue("address1", item.Address1);
                                address.SetValue("address2", item.Address2);
                                address.SetValue("PostalCode", item.PostalCode);
                                address.SetValue("District", item.District);
                                // Get the page using the GUID you've defined

                                if (item.CountryId != 0)
                                {
                                    var content = _contentService.GetById((int)item.CountryId);
                                    // Create an Udi of the page
                                    var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                    address.SetValue("addressCountry", udi.ToString());
                                }
                                _contentService.SaveAndPublish(address);
                                item.Id = address.Id;

                            }

                        }

                        //Add Phone Number for ShareHolder

                        if (shareholder.Phones.Count() > 0)
                        {
                            foreach (var phone in shareholder.Phones)
                            {
                                var objphone = _contentService.Create("phone Number", request.Id, "phone");
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
                            }
                        }
                        return Ok(shareholder);
                    }
                    else
                    {
                        return BadRequest("ShareHolder already exists");
                    }

                }
                else
                    return NotFound("Company Not Founc");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update ShareholderById
        /// <summary>
        /// Update the Shareholder by shareholderId
        /// </summary>
        /// <param name="shareholder"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateShareholder([FromBody] Shareholder shareholder) {
            try
            {

                var query = _contentQuery.Content(shareholder.Id);
               
                    if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("brandName", shareholder.BrandName);
                    toUpdate.SetValue("firstName", shareholder.FirstName);
                    toUpdate.SetValue("lastName", shareholder.LastName);
                    toUpdate.SetValue("middleName", shareholder.MiddleName);
                    toUpdate.SetValue("legalName", shareholder.LegalName);
                    toUpdate.SetValue("email", shareholder.Email);
                    toUpdate.SetValue("shareholderType", shareholder.ShareholderType);
                    toUpdate.SetValue("institutionalType", shareholder.InstituitionalType);
                    toUpdate.SetValue("investorType", shareholder.InvestorType);
                    toUpdate.SetValue("status", shareholder.Status);

                    if (shareholder.CitizenshipCountryId != 0)
                    {
                        var content = _contentService.GetById((int)shareholder.CitizenshipCountryId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                        toUpdate.SetValue("citizenshipCountry", udi.ToString());
                    }
                    if (shareholder.TaxCountryId != 0)
                    {
                        var content = _contentService.GetById((int)shareholder.TaxCountryId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                        toUpdate.SetValue("taxCountry", udi.ToString());
                    }
                  
                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);
                  


                    // Add address for this shareholder
                    if (shareholder.Addresses.Count() > 0)
                    {
                        foreach (var item in shareholder.Addresses)
                        {
                            Umbraco.Cms.Core.Models.IContent address = null;
                            if (item.Id == 0)                            
                                 address = _contentService.Create("Home Address", query.Id, "address");
                           else
                             address = _contentService.GetById(item.Id);
                        
                         
                            address.UpdateDate = DateTime.UtcNow;
                            address.SetValue("city", item.City);
                            address.SetValue("address1", item.Address1);
                            address.SetValue("address2", item.Address2);
                            address.SetValue("PostalCode", item.PostalCode);
                            address.SetValue("District", item.District);
                            // Get the page using the GUID you've defined

                            if (item.CountryId != 0)
                            {
                                var content = _contentService.GetById((int)item.CountryId);
                                // Create an Udi of the page
                                var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                address.SetValue("addressCountry", udi.ToString());
                            }
                            _contentService.SaveAndPublish(address);
                        
                        }

                    }

                    //Add Phone Number for ShareHolder

                    if (shareholder.Phones.Count() > 0)
                    {
                        foreach (var phone in shareholder.Phones)
                        {
                            Umbraco.Cms.Core.Models.IContent objphone = null;
                            if (phone.Id == 0)
                                objphone = _contentService.Create("Phone Number", query.Id, "phone");
                            else
                                objphone = _contentService.GetById(phone.Id);
                           
                          
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
                          
                        }
                    }
                    return Ok(shareholder);
                }
                else
                {
                    return NotFound("Shareholder not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Delete Share Holder By Id
        /// <summary>
        /// Delet share holder by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteShareholder(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "shareholder")
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
