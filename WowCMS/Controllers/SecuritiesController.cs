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
    public class SecuritiesController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;

        public SecuritiesController(IPublishedContentQuery contentQuery, IContentService contentService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
        }


        #region GetAllSecuritiesByCompanyId
        /// <summary>
        /// Get all the Securities
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllSecuritiesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetAllSecuritiesByCompanyId(int companyId) {
            try
            {
                IPublishedContent query = _contentQuery.Content(companyId);
                if (query !=null)
                {
             var Securities=  query.Children().Where(x => x.ContentType.Alias == "captable").FirstOrDefault();
                    if (Securities != null)
                    {
                        List<Securities> results = new List<Securities>();
                        foreach (var item in Securities.Children().OrderBy(x => x.SortOrder))
                        {
                            results.Add(new Securities
                            {
                                Id = item.Id,
                                Name = item.Value<string>("securitiesName"),
                                EqPrefix = item.Value<string>("eQPrefix"),
                                FaceValue = item.Value<float>("faceValue"),
                                PricePerShare = item.Value<float>("pricePerShare"),
                                ExercisePrice = item.Value<int>("exercisePrice"),
                                BoardFirstIssueDate = item.Value<DateTime>("boardFirstIssueDate"),
                                CreatedDate = item.CreateDate,
                                IsVoting = item.Value<bool>("isVoting"),
                                ConversionPrice = item.Value<float>("conversionPrice"),
                                ConversionDiscount = item.Value<float>("conversionDiscount"),
                                ConvertToFundingRound = item.Value<string>("ConvertToFundingRound"),
                                ConvertToPrefix = item.Value<string>("convertToPrefix"),
                                MaturityDate = item.Value<DateTime>("maturityDate"),
                                PreMoneyValuation = item.Value<float>("preMoneyValuation"),
                                Multiplier = item.Value<float>("multiplier"),
                                PreMoneyValuationCap = item.Value<float>("preMoneyValuationCap"),
                                ParticipationCap = item.Value<int>("participationCap"),
                                IsParticipating = item.Value<bool>("isParticipating"),
                                CompanyId = item.Parent.Parent.Id
                            });
                        }
                        return Ok(results);

                    }
                    else
                    return NotFound("No securities Found");
                }else
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region GetSecuritiesById
        /// <summary>
        /// Get Securities by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSecurities(int id) {
            try
            {
                IPublishedContent query = _contentQuery.Content(id);
                if (query !=null && query.ContentType.Alias== "securities")
                {
                    Securities results = new Securities()
                    {
                        Id = query.Id,
                        Name = query.Value<string>("securitiesName"),
                        EqPrefix = query.Value<string>("eQPrefix"),
                        FaceValue = query.Value<float>("faceValue"),
                        PricePerShare = query.Value<float>("pricePerShare"),
                        ExercisePrice = query.Value<int>("exercisePrice"),
                        BoardFirstIssueDate = query.Value<DateTime>("boardFirstIssueDate"),
                        CreatedDate = query.CreateDate,
                        IsVoting = query.Value<bool>("isVoting"),
                        ConversionPrice = query.Value<float>("conversionPrice"),
                        ConversionDiscount = query.Value<float>("conversionDiscount"),
                        ConvertToFundingRound = query.Value<string>("convertToFundingRound"),
                        ConvertToPrefix = query.Value<string>("convertToPrefix"),
                        MaturityDate = query.Value<DateTime>("maturityDate"),
                        PreMoneyValuation = query.Value<float>("preMoneyValuation"),
                        Multiplier = query.Value<float>("multiplier"),
                        PreMoneyValuationCap = query.Value<float>("preMoneyValuationCap"),
                        ParticipationCap = query.Value<int>("participationCap"),
                        IsParticipating = query.Value<bool>("isParticipating"),
                        CompanyId = query.Parent.Parent.Id
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

        #region Add Securities by companyId
        /// <summary>
        /// Add Securities by companyId
        /// </summary>
        /// <param name="securities"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSecurities([FromBody] Securities securities) {
            try
            {
                var company = _contentQuery.Content(securities.CompanyId);
                if (company != null)
                {

                    int captableFolderId = 0;
                    IPublishedContent query = null;
                    IPublishedContent contentRoot = company.Children().Where(x => x.ContentType.Alias.Equals("captable")).FirstOrDefault();
                    if (contentRoot == null)
                    {
                        // add ShareHolder folder
                        var shareholderFolder = _contentService.Create("Captable", company.Id, "captable");
                        shareholderFolder.CreateDate = DateTime.UtcNow;
                        shareholderFolder.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(shareholderFolder);
                        captableFolderId = shareholderFolder.Id;
                    }
                    else
                    {
                        query = contentRoot.Children()
                       .Where(x => x.Value<string>("securitiesName").ToLower() == securities.Name.ToLower())
                       .FirstOrDefault();
                   captableFolderId=     contentRoot.Id;
                    }

                    if (query == null)
                    {
                        var request = _contentService.Create(securities.Name, captableFolderId, "securities");
                        request.SetValue("securitiesName", securities.Name);
                        request.SetValue("eQPrefix", securities.EqPrefix);
                        request.SetValue("faceValue", securities.FaceValue);
                        request.SetValue("pricePerShare", securities.PricePerShare);
                        request.SetValue("exercisePrice", securities.ExercisePrice);
                        request.SetValue("boardFirstIssueDate", securities.BoardFirstIssueDate);
                        request.SetValue("isVoting", securities.IsVoting);
                        request.SetValue("conversionPrice", securities.ConversionPrice);
                        request.SetValue("conversionDiscount", securities.ConversionDiscount);
                        request.SetValue("convertToFundingRound", securities.ConvertToFundingRound);

                        request.SetValue("convertToPrefix", securities.ConvertToPrefix);
                        request.SetValue("maturityDate", securities.MaturityDate);


                        request.SetValue("preMoneyValuation", securities.PreMoneyValuation);
                        request.SetValue("maturityDate", securities.MaturityDate);

                        request.SetValue("multiplier", securities.Multiplier);
                        request.SetValue("preMoneyValuationCap", securities.PreMoneyValuationCap);

                        request.SetValue("participationCap", securities.ParticipationCap);
                        request.SetValue("isParticipating", securities.IsParticipating);


                        request.CreateDate = DateTime.UtcNow;
                        request.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(request);
                        securities.Id = request.Id;


                        return Ok(securities);
                    }
                    else
                    {
                        return BadRequest("ShareHolder already exists");
                    }

                }
                else
                    return NotFound("Company Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region UpdateSecurities
        /// <summary>
        /// Update Securities through Id
        /// </summary>
        /// <param name="securities"></param>
        /// <returns></returns>

        [HttpPut]
        public async Task<IActionResult> UpdateSecurities([FromBody] Securities securities) {
            try
            {

                var query = _contentQuery.Content(securities.Id);

                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("securitiesName", securities.Name);
                    toUpdate.SetValue("eQPrefix", securities.EqPrefix);
                    toUpdate.SetValue("faceValue", securities.FaceValue);
                    toUpdate.SetValue("pricePerShare", securities.PricePerShare);
                    toUpdate.SetValue("exercisePrice", securities.ExercisePrice);
                    toUpdate.SetValue("boardFirstIssueDate", securities.BoardFirstIssueDate);
                    toUpdate.SetValue("isVoting", securities.IsVoting);
                    toUpdate.SetValue("conversionPrice", securities.ConversionPrice);
                    toUpdate.SetValue("conversionDiscount", securities.ConversionDiscount);
                    toUpdate.SetValue("convertToFundingRound", securities.ConvertToFundingRound);

                    toUpdate.SetValue("convertToPrefix", securities.ConvertToPrefix);
                    toUpdate.SetValue("maturityDate", securities.MaturityDate);


                    toUpdate.SetValue("preMoneyValuation", securities.PreMoneyValuation);
                    toUpdate.SetValue("maturityDate", securities.MaturityDate);

                    toUpdate.SetValue("multiplier", securities.Multiplier);
                    toUpdate.SetValue("preMoneyValuationCap", securities.PreMoneyValuationCap);

                    toUpdate.SetValue("participationCap", securities.ParticipationCap);
                    toUpdate.SetValue("isParticipating", securities.IsParticipating);



                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);
                    return Ok(securities);
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

        #region DeleteSecurities
        /// <summary>
        /// Delete Securities by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteSecurities(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "securities")
                {
                    _contentService.Delete(toDelete);
                    return Ok("Successfully Deleted");
                }
                else
                {
                    return NotFound("Not Found");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            #endregion
        }
    }
}
