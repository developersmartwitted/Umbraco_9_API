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
    public class ExerciseRequestController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public ExerciseRequestController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }


        #region GetExerciseRequests
        /// <summary>
        /// Get Exercise Requests Controller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetExerciseRequests() {

            try
            {
                return BadRequest("Not Implemented");

            }
            catch (Exception ex) {
                return BadRequest(ex.Message.ToString());
            }

           
        }
        #endregion

        #region GetExerciseRequest

        /// <summary>
        /// Get Exercise Requests from the Umbraco backoffice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExerciseRequest(int id) 
        {

            try
            {
                return BadRequest("Not Implemented");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
            
        }

        #endregion

        #region AddExerciseRequest

        /// <summary>
        /// Add Excercise Request from Umbraco Backoffice 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddExerciseRequest([FromBody] ExerciseRequest request) {

            try
            {
                return Ok();

            }
            catch (Exception ex) {
                return BadRequest("Not Implemented");

            }

          
        }
        #endregion

        #region UpdateExerciseRequest
        /// <summary>
        /// Update Exercise Request  From through Backoffice 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateExerciseRequest([FromBody] ExerciseRequest request) {
            return BadRequest("Not Implemented");
        }
        #endregion

        #region DeleteExerciseRequest
        /// <summary>
        /// Delete Exercise Request from Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteExerciseRequest(int id) {
            return BadRequest("Not Implemented");
        }

        #endregion
    }
}
