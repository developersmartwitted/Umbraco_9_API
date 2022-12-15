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
    [Route("api/[controller]")]
    [ApiController]
    public class PoolController : UmbracoApiController
    {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public PoolController(IPublishedContentQuery contentQuery, IContentService contentService)
        {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        // no implementation of get all pools

        [HttpGet("ByCompany/{id}")]
        public async Task<IActionResult> GetPoolByCompanyId(int id)
        {
            try
            {
                IPublishedContent query = _contentQuery.Content(id).Children()
                    .Where(x => x.ContentType.Alias == "pool").FirstOrDefault();
                if (query != null)
                {
                    Pool result = new Pool
                    {
                        Id = query.Id,
                        CompanyId = id,
                        UserId = query.WriterId,
                        PoolSize = query.Value<int>("poolSize"),
                        TotalAllocated = query.Value<int>("totalAllocated"),
                        Options = query.Value<int>("options"),
                        RSU = query.Value<int>("rSU"),
                        TotalTransferable = query.Value<int>("totalTransferable"),
                        PoolSetupDate = query.Value<DateTime>("poolSetupDate")
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
        public async Task<IActionResult> AddPool([FromBody] Pool pool)
        {
            try
            {
                if (pool.PoolSize <= 0)
                {
                    return BadRequest("Pool size must have at least 1 unit");
                }
                IPublishedContent company = _contentQuery.Content(pool.CompanyId);
                if (company != null)
                {
                    IPublishedContent query = company.Children()
                        .Where(x => x.ContentType.Alias == "pool").FirstOrDefault();
                    if (query == null)
                    {
                        var request = _contentService.Create("Pool", company.Id, "pool");
                        request.SetValue("poolSize", pool.PoolSize);
                        // changed during grant options
                        request.SetValue("totalAllocated", 0);
                        request.SetValue("options", 0);
                        request.SetValue("rSU", 0);
                        request.SetValue("totalTransferable", 0);
                        request.SetValue("poolSetupDate", pool.PoolSetupDate);
                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;
                        pool.Id = request.Id;
                        _contentService.SaveAndPublish(request);

                        // add history folder to pool
                        var historyFolder = _contentService.Create("History", request.Id, "poolChanges");
                        historyFolder.CreateDate = DateTime.UtcNow;
                        historyFolder.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(historyFolder);

                        // add initial history entry
                        var history = _contentService.Create("Initial Setup", historyFolder.Id, "poolHistory");
                        history.SetValue("poolSize", pool.PoolSize);
                        history.SetValue("action", 1);
                        history.CreateDate = DateTime.UtcNow;
                        history.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(history);

                        // add grants folder to pool
                        var grantsFolder = _contentService.Create("Grants", request.Id, "poolGrants");
                        grantsFolder.CreateDate = DateTime.UtcNow;
                        grantsFolder.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(grantsFolder);

                        return Ok(pool);
                    }
                    else
                    {
                        return BadRequest("Company already has pool setup");
                    }
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
        [HttpPut]
        public async Task<IActionResult> UpdatePool([FromBody] Pool pool)
        {
            return BadRequest("Not Implemented");
        }
        [HttpDelete]
        public async Task<IActionResult> DeletePool(int id)
        {
            try
            {
                IPublishedContent contentRoot = _contentQuery.Content(id);
                IPublishedContent grants = contentRoot?.Children()
                    .Where(x => x.ContentType.Alias == "grants").FirstOrDefault();
                // do not allow pool to be removed when there are grants issued against it
                if (grants?.Children()?.Count() == 0)
                {
                    var toDelete = _contentService.GetById(id);
                    if (toDelete?.ContentType.Alias == "pool")
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
                    return BadRequest("Pool has grants issued against it.  Grants must be cleared before pool can be removed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
