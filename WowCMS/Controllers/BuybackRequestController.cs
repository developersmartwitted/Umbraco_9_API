using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class BuybackRequestController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        public BuybackRequestController(IPublishedContentQuery contentQuery, IContentService contentService,IMediaService mediaService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _mediaService = mediaService;
        }

        #region GetBuybackRequestsByGrantId
        /// <summary>
        /// Get Buy Back Requests
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBuybackRequestsByGrantId/{grantId}")]
        public async Task<IActionResult> GetBuybackRequestsByGrantId(int grantId) {
            try
            {

                var Grant = _contentQuery.Content(grantId);

                if(Grant != null)
                {
                    var query = Grant.Children().Where(x => x.ContentType.Alias.Equals("buybackRequests")).ToList();
                    if (query.Count() > 0)
                    {
                        List<BuybackRequest> results = new List<BuybackRequest>();
                        foreach (var item in query)
                        {
                            var ChildItems = item.Children();
                            if (ChildItems.Count() > 0)
                            {
                                foreach (var ChildItem in ChildItems)
                                {
                                    var typedMediaPickerSingle = ChildItem.Value<IPublishedContent>("transactionProof");

                                    results.Add(new BuybackRequest()
                                    {
                                        Id = ChildItem.Id,
                                        Units = ChildItem.Value<int>("units"),
                                        TransactionDate = ChildItem.Value<DateTime>("transactionDate"),
                                        Status = ChildItem.Value<int>("status"),
                                        RequestDate = ChildItem.Value<DateTime>("requestDate"),
                                        ResponseUnits = ChildItem.Value<int>("responseUnits"),
                                        CreatedDate = ChildItem.CreateDate,
                                        GrantId = ChildItem.Parent.Parent.Parent.Id,
                                        TransactionProof = typedMediaPickerSingle == null ? String.Empty : typedMediaPickerSingle.Url()
                                    });
                                }

                            }

                        }

                        return Ok(results);
                    }
                    return NotFound("Not Found");
                }else
                    return NotFound("No Grant Found");

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        #endregion

        #region GetBuybackRequestByID
        /// <summary>
        /// GetBuybackRequest by Id through Backoffice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuybackRequest(int id) {

            try
            {

                var GetRequests = _contentQuery.Content(id);
                if(GetRequests != null && GetRequests.ContentType.Alias=="buybackRequest")
                {
                    var typedMediaPickerSingle = GetRequests.Value<IPublishedContent>("transactionProof");

                    BuybackRequest results = new BuybackRequest()
                    {
                        Units = GetRequests.Value<int>("units"),
                        TransactionDate = GetRequests.Value<DateTime>("transactionDate"),
                        Status = GetRequests.Value<int>("status"),
                        RequestDate = GetRequests.Value<DateTime>("requestDate"),
                        ResponseUnits = GetRequests.Value<int>("responseUnits"),
                        CreatedDate = GetRequests.CreateDate,
                        GrantId = GetRequests.Parent.Parent.Parent.Id,
                        TransactionProof = typedMediaPickerSingle == null ? String.Empty : typedMediaPickerSingle.Url()

                    };
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

        #region AddBuybackRequest
        /// <summary>
        /// Add BuyBackRequest 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBuybackRequest([FromBody] BuybackRequest buybackrequest) {
            try
            {
                var query = _contentQuery.Content(buybackrequest.GrantId);
                               
                    if (query != null)
                {
                    var BuyRequest = query.Children().Where(x => x.ContentType.Alias.Equals("buybackRequests")).FirstOrDefault();
                    var request = _contentService.Create("BuyBack"+" "+buybackrequest.Units.ToString(), BuyRequest.Id, "buybackRequest");
                    request.SetValue("units", buybackrequest.Units);
                    request.SetValue("requestDate", buybackrequest.RequestDate);
                    request.SetValue("status", buybackrequest.Status);
                    request.SetValue("transactionDate",buybackrequest.TransactionDate);
                    request.SetValue("responseUnits", buybackrequest.ResponseUnits);

                    //Save Image in Folder
                    if (buybackrequest.FormFile.Length > 0)
                    {
                        var CreateImage = _mediaService.CreateMedia(buybackrequest.FormFile.FileName, -1, Constants.Conventions.MediaTypes.Image);
                        CreateImage.SetValue("umbracoFile", buybackrequest.FormFile.OpenReadStream());
                        _mediaService.Save(CreateImage);
                        request.SetValue("transactionProof", CreateImage.GetUdi().ToString());
                    }
             
                    _contentService.SaveAndPublish(request);
                    buybackrequest.Id = request.Id;
                    return Ok(buybackrequest);
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

        #region UpdateBuybackRequest
        /// <summary>
        /// Update BuyBackRequest from backrequest Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateBuybackRequest([FromBody] BuybackRequest request) {
            try
            {
                var query = _contentQuery.Content(request.Id);
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("units", request.Units);
                    toUpdate.SetValue("requestDate", request.RequestDate);
                    toUpdate.SetValue("status", request.Status);
                    toUpdate.SetValue("transactionDate", request.TransactionDate);
                    toUpdate.SetValue("responseUnits", request.ResponseUnits);
                    //Save Image in Folder
                    if (request.FormFile.Length > 0)
                    {
                        var CreateImage = _mediaService.CreateMedia(request.FormFile.FileName, -1, Constants.Conventions.MediaTypes.Image);
                        CreateImage.SetValue("umbracoFile", request.FormFile.OpenReadStream());
                        _mediaService.Save(CreateImage);
                        toUpdate.SetValue("transactionProof", CreateImage.GetUdi().ToString());
                    }

                    _contentService.SaveAndPublish(toUpdate);

                    return Ok(request);
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

        #region Delete Buy Requests
        /// <summary>
        /// Delete Buy Requests By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteBuyRequests(int id)
        {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "buybackRequest")
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
