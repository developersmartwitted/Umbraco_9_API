
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
    public class SecuritiesAllotmentController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public SecuritiesAllotmentController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSecuritiesAllotments() {
            return BadRequest("Not Implemented");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSecuritiesAllotment(int id) {
            return BadRequest("Not Implemented");
        }

        [HttpPost]
        public async Task<IActionResult> AddSecuritiesAllotment([FromBody] SecuritiesAllotment allotment) {
            return BadRequest("Not Implemented");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSecuritiesAllotment([FromBody] SecuritiesAllotment allotment) {
            return BadRequest("Not Implemented");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSecuritiesAllotment(int id) {
            return BadRequest("Not Implemented");
        }
    }
}
