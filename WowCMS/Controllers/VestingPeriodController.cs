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
    public class VestingPeriodController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public VestingPeriodController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        #region GetVestingPeriodByVestingId
        /// <summary>
        /// Get Vesting Period Lists By VestingId
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVestingPeriodByVestingId/{vestingId}")]
        public async Task<IActionResult> GetVestingPeriodByVestingId(int vestingId) {
            try
            {
                var query = _contentQuery.Content(vestingId).Children().ToList();
                if (query.Count() > 0)
                {
                    List<VestingPeriod> results = new List<VestingPeriod>();
                    foreach (var item in query)
                    {
                        results.Add(new VestingPeriod()
                        {
                            VestingSheduleId = item.Parent.Id,
                            VestingPeriodDate = item.Value<DateTime>("vestingDate"),
                            Units = item.Value<int>("vestedUnits"),
                            Status = item.Value<int>("status"),


                        });
                    }

                    return Ok(results);
                }
                return NotFound("Not Found");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        #region GetVestingPeriodById
        /// <summary>
        /// Get Vesting period by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVestingPeriod(int id)
        {

            try
            {
                var vestingPeriod = _contentQuery.Content(id);
          
                if (vestingPeriod != null && vestingPeriod.ContentType.Alias=="vestingPeriod")
                {
                    VestingPeriod results = new VestingPeriod()
                    {
                        VestingSheduleId = vestingPeriod.Parent.Id,
                        VestingPeriodDate = vestingPeriod.Value<DateTime>("vestingDate"),
                        Units = vestingPeriod.Value<int>("vestedUnits"),
                        Status = vestingPeriod.Value<int>("status"),


                    };
                    return Ok(results);
                }
                return NotFound("No Vestind Period Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }

        }


        #endregion

        #region Add Vesting Period
        /// <summary>
        /// Add Vesting period by Vesting ScheduleId
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddVestingPeriod([FromBody] VestingPeriod period) { 
            try
            {
                var query = _contentQuery.Content(period.VestingSheduleId);
                if (query != null)
                {
                    var request = _contentService.Create(period.VestingPeriodDate.ToString("yyyy")+"-"+ period.Units, query.Id, "vestingPeriod");
                    request.SetValue("vestedDate", period.VestingPeriodDate);
                    request.SetValue("vestedUnits", period.Units);
                    request.SetValue("status", period.Status);                 
                    _contentService.SaveAndPublish(request);
                    period.Id = request.Id;
                    return Ok(period);
                }
                else
                    return BadRequest("No Vesting Shedule Found");

            }          
        
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
    }
}
        #endregion

        #region Update Vesting Period
        /// <summary>
        /// Update Vesting Period 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateVestingPeriod([FromBody] VestingPeriod period) {
            try
            {
                var query = _contentQuery.Content(period.Id);
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("vestedDate", period.VestingPeriodDate);
                    toUpdate.SetValue("vestedUnits", period.Units);
                    toUpdate.SetValue("status", period.Status);
                    _contentService.SaveAndPublish(toUpdate);
                    
                    return Ok(period);
                }
                else
                    return BadRequest("No Vesting Period Found");

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        #region Delete Vesting Period
        /// <summary>
        /// Delete Vesting Period By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteVestingPeriod(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "vestingPeriod")
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
