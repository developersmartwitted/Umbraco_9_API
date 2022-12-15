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
    public class VestingScheduleController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IMemberService _memberService;
        public VestingScheduleController(IPublishedContentQuery contentQuery, IContentService contentService, IMemberService memberService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _memberService = memberService;
        }


        #region Get Vesting Schedules By GrantId
        /// <summary>
        /// Get Vesting Schedules by GrantIdfrom the Umbraco bakoffice
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVestingSchedulesByGrantId/{GrantId}")]
        public async Task<IActionResult> GetVestingSchedulesByGrantId(int GrantId) {


            try
            {

                var Grant = _contentQuery.Content(GrantId);

                if (Grant != null)
                {
                    var query = Grant.Children().Where(x => x.ContentType.Alias.Equals("vesting")).ToList();

                    if (query.Count() > 0)
                    {
                        List<VestingSchedule> results = new List<VestingSchedule>();
                        foreach (var item in query)
                        {
                            IPublishedContent employee = null;
                            if (Grant.ContentType.Alias == "grant")
                            {
                                employee = Grant.HasValue("employee") ? Grant.Value("employee") as IPublishedContent : null;
                            }

                            results.Add(new VestingSchedule()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                CliffDays = item.Value<int>("cliffDays"),
                                Note = item.Value<string>("vestingnote"),
                                Status = item.Value<int>("status"),
                                VestingSegments = item.Value<int>("vestingSegments"),
                                IntervalType = item.Value<int>("intervalType"),
                                EmployedExercisePeriod = item.Value<int>("employedExercisePeriod"),
                                EmploymentEndedType = item.Value<int>("employmentEndedType"),
                                ResignedExercisePeriod = item.Value<int>("resignedExercisePeriod"),
                                SegmentInterval = item.Value<int>("segmentInterval"),
                                TotalUnvestedUnits = item.Value<int>("totalUnvestedUnits"),
                                VestingStartDate = item.Value<DateTime>("vestingStartDate"),
                                FixedDurationEnd = item.Value<DateTime>("fixedDurationEnd"),
                                TotalExercisedUnits = item.Value<int>("totalExercisedUnits"),
                                TotalVestedUnits = item.Value<int>("totalVestedUnits"),
                                GrantId = item.Parent.Parent.Id,
                                EmployeeId = employee == null ? 0 : employee.Id
                            });

                        }

                        return Ok(results);
                    }
                    return NotFound("Not Found");
                }
                else
                    return NotFound("No Grant Found");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
           
        }
        #endregion

        #region Get Vesting Schedule By Id

        /// <summary>
        /// Get VestingSchedule by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVestingScheduleById(int id) {
            try
            {

                var vestingShedule = _contentQuery.Content(id);
          
                if (vestingShedule !=null && vestingShedule.ContentType.Alias=="vesting")
                {
                    var employee = vestingShedule.HasValue("employee") ? vestingShedule.Value("employee") as IPublishedContent : null;

                    VestingSchedule results = new VestingSchedule()
                    {
                        Id = vestingShedule.Id,
                        Name = vestingShedule.Name,
                        CliffDays = vestingShedule.Value<int>("cliffDays"),
                        Note = vestingShedule.Value<string>("vestingnote"),
                        Status = vestingShedule.Value<int>("status"),
                        VestingSegments = vestingShedule.Value<int>("vestingSegments"),
                        IntervalType = vestingShedule.Value<int>("intervalType"),
                        EmployedExercisePeriod = vestingShedule.Value<int>("employedExercisePeriod"),
                        EmploymentEndedType = vestingShedule.Value<int>("employmentEndedType"),
                        ResignedExercisePeriod = vestingShedule.Value<int>("resignedExercisePeriod"),
                        SegmentInterval = vestingShedule.Value<int>("segmentInterval"),
                        TotalUnvestedUnits = vestingShedule.Value<int>("totalUnvestedUnits"),
                        VestingStartDate = vestingShedule.Value<DateTime>("vestingStartDate"),
                        FixedDurationEnd = vestingShedule.Value<DateTime>("fixedDurationEnd"),
                        TotalExercisedUnits = vestingShedule.Value<int>("totalExercisedUnits"),
                        TotalVestedUnits = vestingShedule.Value<int>("totalVestedUnits"),
                        GrantId=vestingShedule.Parent.Parent.Id,
                        EmployeeId=employee==null?0:  employee.Id                       

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

        #region AddVestingSchedule
        /// <summary>
        /// Add the Vesting Schedule
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddVestingSchedule([FromBody] VestingSchedule schedule) {

            try
            {
                var query = _contentQuery.Content(schedule.GrantId);

                
                    if (query != null)
                {
                    var Isvestingexits = query.Children().Where(x => x.Name.ToLower().Equals(schedule.Name.ToLower())).FirstOrDefault();
                    if (Isvestingexits == null)
                    {
                        var request = _contentService.Create(schedule.Name, query.Id, "vesting");
                        request.SetValue("cliffDays", schedule.CliffDays);
                        request.SetValue("vestingnote", schedule.Note);
                        request.SetValue("status", schedule.Status);
                        request.SetValue("intervalType", schedule.IntervalType);
                        request.SetValue("vestingSegments", schedule.VestingSegments);
                        request.SetValue("employedExercisePeriod", schedule.EmployedExercisePeriod);
                        request.SetValue("employmentEndedType", schedule.EmploymentEndedType);
                        request.SetValue("resignedExercisePeriod", schedule.ResignedExercisePeriod);
                        request.SetValue("segmentInterval", schedule.SegmentInterval);
                        request.SetValue("totalUnvestedUnits", schedule.TotalUnvestedUnits);
                        request.SetValue("vestingStartDate", schedule.VestingStartDate);
                        request.SetValue("fixedDurationEnd", schedule.FixedDurationEnd);
                        request.SetValue("totalExercisedUnits", schedule.TotalExercisedUnits);
                        request.SetValue("totalVestedUnits", schedule.TotalVestedUnits);

                        if (schedule.EmployeeId != 0)
                        {
                            var content = _memberService.GetById((int)schedule.EmployeeId);
                            // Create an Udi of the page
                            var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                            request.SetValue("employee", udi.ToString());
                        }
                        _contentService.SaveAndPublish(request);
                        schedule.Id = request.Id;
                        return Ok(schedule);
                    }
                    else
                        return BadRequest("Vesting Already Exists ");

                }
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
           
        }

        #endregion

        #region UpdateVestingSchedule
        /// <summary>
        /// update the Vesting Schedule
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>

        [HttpPut]
        public async Task<IActionResult> UpdateVestingSchedule([FromBody] VestingSchedule schedule) {
            try
            {
                var query = _contentQuery.Content(schedule.Id);

               
                if (query != null)
                {

                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("cliffDays", schedule.CliffDays);
                    toUpdate.SetValue("vestingnote", schedule.Note);
                    toUpdate.SetValue("status", schedule.Status);
                    toUpdate.SetValue("intervalType", schedule.IntervalType);
                    toUpdate.SetValue("vestingSegments", schedule.VestingSegments);
                    toUpdate.SetValue("employedExercisePeriod", schedule.EmployedExercisePeriod);
                    toUpdate.SetValue("employmentEndedType", schedule.EmploymentEndedType);
                    toUpdate.SetValue("resignedExercisePeriod", schedule.ResignedExercisePeriod);
                    toUpdate.SetValue("segmentInterval", schedule.SegmentInterval);
                    toUpdate.SetValue("totalUnvestedUnits", schedule.TotalUnvestedUnits);
                    toUpdate.SetValue("vestingStartDate", schedule.VestingStartDate);
                    toUpdate.SetValue("fixedDurationEnd", schedule.FixedDurationEnd);
                    toUpdate.SetValue("totalExercisedUnits", schedule.TotalExercisedUnits);
                    toUpdate.SetValue("totalVestedUnits", schedule.TotalVestedUnits);

                    if (schedule.EmployeeId != 0)
                    {
                        var content = _memberService.GetById((int)schedule.EmployeeId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                        toUpdate.SetValue("employee", udi.ToString());
                    }
                    _contentService.SaveAndPublish(toUpdate);
                
                    return Ok(schedule);
                }
                return NotFound("Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        #endregion

        #region DeleteVestingSchedule
        /// <summary>
        /// Delete the vesting Schedule thorugh Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteVestingSchedule(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "vesting")
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
