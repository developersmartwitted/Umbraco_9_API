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
    public class CountryController : UmbracoApiController
    {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public CountryController(IPublishedContentQuery contentQuery, IContentService contentService)
        {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                IEnumerable<IPublishedContent> query = _contentQuery.ContentSingleAtXPath("//countries").Children().OrderBy(x => x.SortOrder);                
                if (query.Any())
                {
                    List<Country> results = new List<Country>();
                    foreach (var item in query)
                    {
                        results.Add(new Country
                        {
                            Id = item.Id,
                            Name = item.Value<string>("countryName"),
                            PhonePrefix = item.Value<string>("phonePrefix")
                        });
                    }
                    return Ok(results);
                }
                return NotFound();
            } catch (Exception ex) {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(id);
                if (query?.ContentType.Alias == "country")
                {
                    Country result = new Country
                    {
                        Id = id,
                        Name = query.Value<string>("countryName"),
                        PhonePrefix = query.Value<string>("phonePrefix")
                    };
                    return Ok(result);
                }
                return NotFound();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCountry([FromBody] Country country)
        {
            try
            {
                country.Name = country.Name.Trim();
                string searchFor = country.Name.ToUpper();
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//countries");
                IPublishedContent query = contentRoot.Children()
                    .Where(x => x.Value<string>("countryName").ToUpper() == searchFor)
                    .FirstOrDefault();
                if (query == null)
                {
                    var request = _contentService.Create(country.Name, contentRoot.Id, "country");
                    request.SetValue("countryName", country.Name);
                    request.SetValue("phonePrefix", country.PhonePrefix);
                    request.CreateDate = DateTime.UtcNow;
                    request.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(request);
                    country.Id = request.Id;
                    return Ok(country);
                }
                else
                {
                    return BadRequest("Country already exists");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromBody] Country country)
        {
            try
            {
                country.Name = country.Name.Trim();
                string searchFor = country.Name.ToUpper();
                IPublishedContent contentRoot = _contentQuery.ContentSingleAtXPath("//countries");
                IPublishedContent query = contentRoot.Children()
                    .Where(x => x.Value<string>("countryName").ToUpper() == searchFor || x.Id == country.Id)
                    .FirstOrDefault();
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    if (country.Name != null) toUpdate.SetValue("countryName", country.Name);
                    if (country.PhonePrefix != null) toUpdate.SetValue("phonePrefix", country.PhonePrefix);
                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);
                    country.Id = toUpdate.Id;
                    return Ok(country);
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
        public async Task<IActionResult> DeleteCountry(int id)
        {
            try
            {
                var toDelete = _contentService.GetById(id);
                if (toDelete?.ContentType.Alias == "country")
                {
                    _contentService.Delete(toDelete);
                    return Ok();
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
    }
}
