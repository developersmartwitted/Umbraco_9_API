using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
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
    public class GrantController : UmbracoApiController
    {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IMemberService _memberService;

        public GrantController(IPublishedContentQuery contentQuery,
            IMemberService memberService,
            IContentService contentService)
        {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _memberService = memberService;
        }

        [HttpGet("GrantsByPool/{id}")]
        public async Task<IActionResult> GetGrantsByPool(int id)
        {
            try
            {
                IPublishedContent contentRoot = _contentQuery.Content(id);
                if (contentRoot?.ContentType.Alias == "pool")
                {
                    var query = contentRoot.DescendantsOfType("grant");
                    if (query != null)
                    {
                        var companyId = contentRoot.Ancestors()
                            .Where(x => x.ContentType.Alias == "company").FirstOrDefault().Id;
                        List<Grant> results = new List<Grant>();
                        foreach (var item in query)
                        {
                            var exercisePeriodId = item.DescendantOfType("exercisePeriod")?.Id;
                            var employee = item.GetProperty("employee").GetValue() as IPublishedContent;
                            results.Add(new Grant
                            {
                                Id = item.Id,
                                EmployeeId = employee.Id,
                                PoolId = id,
                                CompanyId = companyId,
                                SecurityType = item.Value<int>("securityType"),
                                // VestingScheduleId = 
                                GrantDate = item.Value<DateTime>("grantDate"),
                                VotingType = item.Value<int>("votingType"),
                                VestingStartDate = item.Value<DateTime>("vestingStartDate"),
                                VestingCommencementDate = item.Value<DateTime>("vestingCommencementDate"),
                                Units = item.Value<int>("units"),
                                ExercisePricePerOption = item.Value<float>("exercisePricePerOption"),
                                // BoardResolutionDocument = 
                                Note = item.Value<string>("note"),
                                Status = item.Value<int>("status")
                            });
                        }
                        return Ok(results);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GrantsByCompany/{id}")]
        public async Task<IActionResult> GetGrantsByCompany(int id)
        {
            try
            {
                IPublishedContent contentRoot = _contentQuery.Content(id);
                if (contentRoot?.ContentType.Alias == "company")
                {
                    var query = contentRoot.DescendantsOfType("grant");
                    if (query != null)
                    {
                        var poolId = contentRoot.DescendantOfType("pool").Id;
                        List<Grant> results = new List<Grant>();
                        foreach (var item in query)
                        {
                            var exercisePeriodId = item.DescendantOfType("exercisePeriod")?.Id;
                            var employee = item.GetProperty("employee").GetValue() as IPublishedContent;
                            results.Add(new Grant
                            {
                                Id = item.Id,
                                PoolId = poolId,
                                EmployeeId = employee.Id,
                                CompanyId = id,
                                SecurityType = item.Value<int>("securityType"),
                                // VestingId = 
                                GrantDate = item.Value<DateTime>("grantDate"),
                                VotingType = item.Value<int>("votingType"),
                                VestingStartDate = item.Value<DateTime>("vestingStartDate"),
                                VestingCommencementDate = item.Value<DateTime>("vestingCommencementDate"),
                                Units = item.Value<int>("units"),
                                ExercisePricePerOption = item.Value<float>("exercisePricePerOption"),
                                // BoardResolutionDocument = 
                                Note = item.Value<string>("note"),
                                Status = item.Value<int>("status")
                            });
                        }
                        return Ok(results);
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddGrant([FromBody] Grant grant)
        {
            try
            {
                IPublishedContent pool = _contentQuery.Content(grant.PoolId);
                if (pool != null)
                {
                    var grants = pool.DescendantOfType("poolGrants");
                    int poolSize = pool.Value<int>("poolSize");
                    int totalAllocated = pool.Value<int>("totalAllocated");
                    // must have enough available units in pool to add grant
                    /*
                     * TODO: change this to allocate more units to poolsize if not large enough
                     */
                    if (poolSize - (totalAllocated + grant.Units) < 0)
                    {
                        return BadRequest("Not enough units in pool to grant");
                    }
                    var employee = _contentQuery.Content(grant.CompanyId).DescendantsOfType("employee")
                        .Where(x => x.Id == grant.EmployeeId).FirstOrDefault();
                    //var employee = _contentQuery.Content(grant.EmployeeId);
                    string grantName = employee.Value<string>("fullName") + " Grant";
                    var request = _contentService.Create(grantName, grants.Id, "grant");
                    request.SetValue("securityType", grant.SecurityType);
                    request.SetValue("grantDate", grant.GrantDate);
                    request.SetValue("employee", employee);
                    /*
                     * TODO: employee not properly being assigned in this manner
                     */
                    request.SetValue("votingType", grant.VotingType);
                    request.SetValue("vestingStartDate", grant.VestingStartDate);
                    request.SetValue("vestingCommencementDate", grant.VestingCommencementDate);
                    request.SetValue("units", grant.Units);
                    request.SetValue("exercisePricePerOption", grant.ExercisePricePerOption);
                    // request.SetValue("boardResolutionDocument", grant.BoardResolutionDocument);
                    request.SetValue("note", grant.Note);
                    request.SetValue("status", grant.Status);
                    request.CreateDate = DateTime.UtcNow;
                    request.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(request);
                    grant.Id = request.Id;

                    // update pool totals
                    var updatePool = _contentService.GetById(grant.PoolId);
                    updatePool.SetValue("totalAllocated", totalAllocated + grant.Units);
                    if (grant.SecurityType == 0) // options
                    {
                        int totalOptions = pool.Value<int>("options");
                        updatePool.SetValue("options", totalOptions + grant.Units);
                    }
                    else // RSU
                    {
                        int totalRSU = pool.Value<int>("rSU");
                        updatePool.SetValue("rSU", totalRSU + grant.Units);
                    }
                    _contentService.SaveAndPublish(updatePool);
                    return Ok(grant);
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
        public async Task<IActionResult> UpdateGrant([FromBody] Grant grant)
        {
            /*
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
                    return BadRequest("Company does not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            */
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGrant(int id)
        {
            /*
            try
            {
                IPublishedContent contentRoot = _contentQuery.Content(id);
                // do not allow company removal if there is a pool assigned or employees assigned to it
                // this ensures we do not have people using the system that are part of a non-existant company
                if (contentRoot?.Children()?.Count() == 0)
                {
                    var toDelete = _contentService.GetById(id);
                    if (toDelete.ContentType.Alias == "company")
                    {
                        _contentService.Delete(toDelete);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Company does not exist");
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
            */
            return Ok();
        }
    }
}
