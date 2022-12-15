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
    public class PoolHistoryController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IMemberService _memberService;
        public PoolHistoryController(IPublishedContentQuery contentQuery, IContentService contentService, IMemberService memberService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _memberService = memberService;
        }

        #region GetPoolHistoriesByPoolID
        /// <summary>
        /// Get GetPoolHistoriesByPoolID through Umbraco Backoffice
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPoolHistoriesByPoolID/{poolId}")]
        public async Task<IActionResult> GetPoolHistoriesByPoolID(int poolId)
        {
            try
            {
                var GetPool = _contentQuery.Content(poolId);
                if (GetPool != null)
                {
                    IPublishedContent query = GetPool.Children().Where(x => x.ContentType.Alias.Equals("poolChanges")).FirstOrDefault();
                    if (query !=null)
                    {
                        List<PoolHistory> results = new List<PoolHistory>();
                        foreach (var item in query.Children().OrderBy(x=>x.SortOrder))
                        {
                            var member = item.HasValue("modifiedBy") ? item.Value<IPublishedContent>("modifiedBy") : null;

                            results.Add(new PoolHistory
                            {
                                Id = item.Id,
                                PoolSize = item.Value<int>("poolSize"),
                                Action = item.Value<int>("action"),
                                UserId = member == null ? 0 : member.Id
                            });


                        }
                        return Ok(results);
                    }
                    return NotFound("Not Found");

                }
                else
                    return NotFound("No Pool Found");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get PoolHistory By Id
        /// <summary>
        /// Get PoolHistoryById through Umbraco Backoffice
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
      
        public async Task<IActionResult> GetPoolHistoryById(int id)
        {
            try
            {
               var query = _contentQuery.Content(id);
                if (query !=null && query.ContentType.Alias=="poolHistory")
                {
                    var member = query.HasValue("modifiedBy") ? query.Value<IPublishedContent>("modifiedBy") : null;

                    PoolHistory results = new PoolHistory()
                    {
                        Id = query.Id,
                        PoolSize = query.Value<int>("poolSize"),
                        Action = query.Value<int>("action"),
                        UserId = member == null ? 0 : member.Id
                    };
                            
                    return Ok(results);
                }
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region AddPoolHistory
        /// <summary>
        /// Add Pool History 
        /// </summary>
        /// <param name="poolHistory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPoolHistory([FromBody] PoolHistory poolHistory) {
            try
            {

                IPublishedContent query = _contentQuery.Content(poolHistory.Id);
              
                if (query == null)
                {

                    var history = _contentService.Create("Initial Setup", poolHistory.poolChangeId, "poolHistory");
                    history.SetValue("poolSize", poolHistory.PoolSize);
                    history.SetValue("action", poolHistory.Action);
                    if (poolHistory.UserId != 0)
                    {
                        var content = _memberService.GetById((int)poolHistory.UserId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                        history.SetValue("modifiedBy", udi.ToString());
                    }
                    history.CreateDate = DateTime.UtcNow;
                    history.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(history);
                    poolHistory.Id = history.Id;
                    return Ok(poolHistory);
                }
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
                    
        }
        #endregion

        #region UpdatePoolHistory
        /// <summary>
        /// Update Pool History By Pool Id
        /// </summary>
        /// <param name="poolHistory"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePoolHistory([FromBody] PoolHistory poolHistory) {
            try
            {

                IPublishedContent query = _contentQuery.Content(poolHistory.Id);

                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("poolSize", poolHistory.PoolSize);
                    toUpdate.SetValue("action", poolHistory.Action);
                    if (poolHistory.UserId != 0)
                    {
                        var content = _memberService.GetById((int)poolHistory.UserId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                        toUpdate.SetValue("modifiedBy", udi.ToString());
                    }
                    
                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);
                    return Ok(poolHistory);
                }
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        #region DeletePoolHistory


        /// <summary>
        /// Delete Pool History bu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeletePoolHistory(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "poolHistory")
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
