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

namespace WowCMS.Controllers
{
    [ApiController]
    [Route("umbraco/api/[controller]")]
    public class CompanyController : UmbracoApiController
    {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public CompanyController(IPublishedContentQuery contentQuery, IContentService contentService)
        {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                IEnumerable<IPublishedContent> query = _contentQuery.ContentSingleAtXPath("//companies").Children().OrderBy(x => x.SortOrder);
                if (query.Any())
                {
                    List<Company> results = new List<Company>();
                    foreach (var item in query)
                    {
                        results.Add(new Company
                        {
                            Id = item.Id,
                            Name = item.Value<string>("companyName")
                        });
                    }
                    return Ok(results);
                }
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(id);
                if (query != null)
                {
                    if (query.ContentType.Alias != "company")
                    {
                        return NotFound();
                    }
                    Company result = new Company
                    {
                        Id = id,
                        Name = query.Value<string>("companyName")
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
        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] Company company)
        {
            try
            {
                company.Name = company.Name.Trim();
                string searchFor = company.Name.ToUpper();
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//companies");
                IPublishedContent query = contentRoot.Children()
                    .Where(x => x.Value<string>("companyName").ToUpper() == searchFor)
                    .FirstOrDefault();
                if (query == null)
                {
                    var request = _contentService.Create(company.Name, contentRoot.Id, "company");
                    request.SetValue("companyName", company.Name);
                    request.CreateDate = DateTime.UtcNow;
                    request.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(request);
                    company.Id = request.Id;

                    // add employees folder
                    var employeeFolder = _contentService.Create("Employees", request.Id, "employees");
                    employeeFolder.CreateDate = DateTime.UtcNow;
                    employeeFolder.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(employeeFolder);


                    // add shareholders folder
                    var shareholdersFolder = _contentService.Create("Shareholders", request.Id, "shareholders");
                    shareholdersFolder.CreateDate = DateTime.UtcNow;
                    shareholdersFolder.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(shareholdersFolder);



                    // add Captable folder
                    var CaptableFolder = _contentService.Create("Captable", request.Id, "captable");
                    CaptableFolder.CreateDate = DateTime.UtcNow;
                    CaptableFolder.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(CaptableFolder);

                    return Ok(company);
                }
                else
                {
                    return BadRequest("Company already exists");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] Company company)
        {
            try
            {
                company.Name = company.Name.Trim();
                string searchFor = company.Name.ToUpper();
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//companies");
                IPublishedContent query = contentRoot.Children()
                    .Where(x => x.Value<string>("companyName").ToUpper() == searchFor || x.Id == company.Id)
                    .FirstOrDefault();
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    if (company.Name != null) toUpdate.SetValue("companyName", company.Name);
                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);
                    company.Id = toUpdate.Id;
                    return Ok(company);
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
        [HttpDelete]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                IPublishedContent contentRoot = _contentQuery.Content(id);
                IPublishedContent employee = contentRoot?.Children()
                    .Where(x => x.ContentType.Alias == "employee").FirstOrDefault();
                IPublishedContent pool = contentRoot?.Children()
                    .Where(x => x.ContentType.Alias == "pool").FirstOrDefault();
                // do not allow company removal if there is a pool assigned or employees assigned to it
                // this ensures we do not have people using the system that are part of a non-existant company
                if (employee == null && pool == null)
                {
                    var toDelete = _contentService.GetById(id);
                    if (toDelete.ContentType.Alias == "company")
                    {
                        _contentService.Delete(toDelete);
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return BadRequest("Company has employees or pool and cannot be removed until employees and pool have been cleared");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
