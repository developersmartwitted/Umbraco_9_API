using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;
using WowCMS.Helpers;
using WowsGlobal.Models;

namespace WowCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : UmbracoApiController {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IContentService _contentService;
        private readonly IMemberService _memberService;
        public EmployeeController(IPublishedContentQuery contentQuery, IContentService contentService,IMemberService memberService) {
            _contentQuery = contentQuery;
            _contentService = contentService;
            _memberService = memberService;
        }

        #region Get Employees By CompanyId
        /// <summary>
        /// Get Employee by CompanyId from the umbraco CMS
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEmployeesByCompanyId/{companyId}")]
        public async Task<IActionResult> GetEmployeesByCompanyId(int companyId) {

            try
            {

                var GetCompany = _contentQuery.Content(companyId);
                if (GetCompany != null)
                {
                    IPublishedContent query = GetCompany.Children().Where(x => x.ContentType.Alias.Equals("employees")).FirstOrDefault();
                    if (query != null)
                    {
                        List<Employee> results = new List<Employee>();
                        foreach (var item in query.Children().OrderBy(x => x.SortOrder))
                        {
                            var member = item.HasValue("memberID") ? item.Value<IPublishedContent>("memberID") : null;
                            results.Add(new Employee
                            {
                                Id = item.Id,
                                EmployeeId = item.Value<string>("employeeId"),
                                FullName = item.Value<string>("fullName"),
                                JoiningDate = item.Value<DateTime>("joiningDate"),
                                //UserId=item.Value<string>("memberId"),
                                Status = item.Value<int>("status"),
                                Designation = item.Value<string>("designation"),
                                Department = item.Value<string>("department"),
                                ImportId = item.Value<int>("importID"),
                                Note = item.Value<string>("note"),
                                Subsidiary = item.Value<string>("subsidiary"),
                                UserId = member == null ? 0 : member.Id
                            });
                        }
                        return Ok(results);
                    }
                    return NotFound("No employees Found For this company");
                }else
                    return NotFound("No Company found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get EmployeeById
        /// <summary>
        /// Get Employee by Id from Umbraco CMS
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id) {
            try
            {
                var query = _contentQuery.Content(id);
                if (query !=null && query.ContentType.Alias=="employee")
                {                    
                   var member = query.HasValue("memberID") ? query.Value<IPublishedContent>("memberID") : null;
                    var employee = new Employee()
                    {

                        Id = query.Id,
                        EmployeeId = query.Value<string>("employeeId"),
                        FullName = query.Value<string>("fullName"),
                        JoiningDate = query.Value<DateTime>("joiningDate"),
                        //UserId=item.Value<string>("memberId"),
                        Status = query.Value<int>("status"),
                        Designation = query.Value<string>("designation"),
                        Department = query.Value<string>("department"),
                        ImportId = query.Value<int>("importID"),
                        Note = query.Value<string>("note"),
                        Subsidiary = query.Value<string>("subsidiary"),
                        UserId = member == null ? 0 : member.Id
                    };
                    
                    return Ok(employee);
                }
                return NotFound("Employee not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Add Employee
        /// <summary>
        /// Add Employee under company in umbraco cms
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee) {
            try
            {
                var GetCompany = _contentQuery.Content(employee.CompanyId);
                if (GetCompany != null)
                {
                    int employeeFolderId = 0;
                    IPublishedContent query = null;
                    IPublishedContent contentRoot = GetCompany.Children().Where(x=>x.ContentType.Alias.Equals("employees")).FirstOrDefault();
                    if(contentRoot == null)
                    {
                        // add employees folder
                        var employeeFolder = _contentService.Create("Employees", GetCompany.Id, "employees");
                        employeeFolder.CreateDate = DateTime.UtcNow;
                        employeeFolder.UpdateDate = DateTime.UtcNow;
                        _contentService.SaveAndPublish(employeeFolder);
                        employeeFolderId = employeeFolder.Id;
                    }
                    else
                    {
                        query = contentRoot.Children()
                           .Where(x => x.Value<string>("fullName").ToLower() == employee.FullName.ToLower())
                           .FirstOrDefault();
                        employeeFolderId = contentRoot.Id;
                    }
                         
                        if (query == null)
                        {
                            var request = _contentService.Create(employee.FullName, employeeFolderId, "employee");
                            request.SetValue("fullName", employee.FullName);
                            request.SetValue("note", employee.Note);
                            request.SetValue("joiningDate", employee.JoiningDate.ToString());
                            request.SetValue("importID", employee.ImportId);

                            if (employee.Status == (int)GenericMethods.Status.Unassigned)
                            {
                                request.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Unassigned.ToString() }));
                            }
                            if (employee.Status == (int)GenericMethods.Status.Active)
                            {
                                request.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Active.ToString() }));
                            }
                            if (employee.Status == (int)GenericMethods.Status.Inactive)
                            {
                                request.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Inactive.ToString() }));
                            }
                            if (employee.Status == (int)GenericMethods.Status.Exited)
                            {
                                request.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Exited.ToString() }));
                            }

                            request.SetValue("subsidiary", employee.Subsidiary);
                            request.SetValue("department", employee.Department);
                            request.SetValue("designation", employee.Designation);


                            if (employee.UserId != 0)
                            {
                                var content = _memberService.GetById((int)employee.UserId);
                                // Create an Udi of the page
                                var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                                request.SetValue("memberID", udi.ToString());
                            }
                            request.CreateDate = DateTime.UtcNow;
                            request.UpdateDate = DateTime.UtcNow;
                            _contentService.SaveAndPublish(request);
                            employee.Id = request.Id;
                            // Add address for this shareholder
                            if (employee.Addresses.Count() > 0)
                            {
                                foreach (var item in employee.Addresses)
                                {
                                    var address = _contentService.Create("Home Address", request.Id, "address");
                                    address.CreateDate = DateTime.UtcNow;
                                    address.UpdateDate = DateTime.UtcNow;
                                    address.SetValue("city", item.City);
                                    address.SetValue("address1", item.Address1);
                                    address.SetValue("address2", item.Address2);
                                    address.SetValue("PostalCode", item.PostalCode);
                                    address.SetValue("District", item.District);
                                    // Get the page using the GUID you've defined

                                    if (item.CountryId != 0)
                                    {
                                        var content = _contentService.GetById((int)item.CountryId);
                                        // Create an Udi of the page
                                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                        address.SetValue("addressCountry", udi.ToString());
                                    }
                                    _contentService.SaveAndPublish(address);
                                    item.Id = address.Id;

                                }

                            }

                            //Add Phone Number for ShareHolder

                            if (employee.Phones.Count() > 0)
                            {
                                foreach (var phone in employee.Phones)
                                {
                                    var objphone = _contentService.Create("phone Number", request.Id, "phone");
                                    objphone.CreateDate = DateTime.UtcNow;
                                    objphone.UpdateDate = DateTime.UtcNow;
                                    objphone.SetValue("phoneNumber", phone.PhoneNumber);
                                    if (phone.CountryId != 0)
                                    {
                                        var content = _contentService.GetById((int)phone.CountryId);
                                        // Create an Udi of the page
                                        var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                        objphone.SetValue("phoneCountry", udi.ToString());
                                    }
                                    _contentService.SaveAndPublish(objphone);
                                    phone.Id = objphone.Id;
                                }
                            }

                            return Ok(employee);
                        }

                    

                    return NotFound();

                }else
                    return NotFound("No Company Found");



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
           
            }
        #endregion

        #region Update Employee

        /// <summary>
        /// Update employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee) {
            try
            {

                IPublishedContent query = _contentQuery.Content(employee.Id);
                if (query != null)
                {
                    var toUpdate = _contentService.GetById(query.Id);
                    toUpdate.SetValue("fullName", employee.FullName);
                    toUpdate.SetValue("note", employee.Note);
                    toUpdate.SetValue("joiningDate", employee.JoiningDate.ToString());
                    toUpdate.SetValue("importID", employee.ImportId);

                    if (employee.Status == (int)GenericMethods.Status.Unassigned)
                    {
                        toUpdate.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Unassigned.ToString() }));
                    }
                    if (employee.Status == (int)GenericMethods.Status.Active)
                    {
                        toUpdate.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Active.ToString() }));
                    }
                    if (employee.Status == (int)GenericMethods.Status.Inactive)
                    {
                        toUpdate.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Inactive.ToString() }));
                    }
                    if (employee.Status == (int)GenericMethods.Status.Exited)
                    {
                        toUpdate.SetValue("status", JsonConvert.SerializeObject(new[] { GenericMethods.Status.Exited.ToString() }));
                    }

                    toUpdate.SetValue("subsidiary", employee.Subsidiary);
                    toUpdate.SetValue("department", employee.Department);
                    toUpdate.SetValue("designation", employee.Designation);


                    if (employee.UserId != 0)
                    {
                        var content = _memberService.GetById((int)employee.UserId);
                        // Create an Udi of the page
                        var udi = Udi.Create(Constants.UdiEntityType.Member, content.Key);
                        toUpdate.SetValue("memberID", udi.ToString());
                    }
                    toUpdate.CreateDate = DateTime.UtcNow;
                    toUpdate.UpdateDate = DateTime.UtcNow;
                    _contentService.SaveAndPublish(toUpdate);

                    // Add address for this shareholder
                    if (employee.Addresses.Count() > 0)
                    {
                        foreach (var item in employee.Addresses)
                        {
                            Umbraco.Cms.Core.Models.IContent address = null;
                            if (item.Id == 0)
                                address = _contentService.Create("Home Address", query.Id, "address");
                            else
                                address = _contentService.GetById(item.Id);

                            address.CreateDate = DateTime.UtcNow;
                            address.UpdateDate = DateTime.UtcNow;
                            address.SetValue("city", item.City);
                            address.SetValue("address1", item.Address1);
                            address.SetValue("address2", item.Address2);
                            address.SetValue("PostalCode", item.PostalCode);
                            address.SetValue("District", item.District);
                            // Get the page using the GUID you've defined

                            if (item.CountryId != 0)
                            {
                                var content = _contentService.GetById((int)item.CountryId);
                                // Create an Udi of the page
                                var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                address.SetValue("addressCountry", udi.ToString());
                            }
                            _contentService.SaveAndPublish(address);

                        }

                    }

                    //Add Phone Number for ShareHolder

                    if (employee.Phones.Count() > 0)
                    {
                        foreach (var phone in employee.Phones)
                        {
                            Umbraco.Cms.Core.Models.IContent objphone = null;
                            if (phone.Id == 0)
                                objphone = _contentService.Create("Phone Number", query.Id, "phone");
                            else
                                objphone = _contentService.GetById(phone.Id);

                            objphone.CreateDate = DateTime.UtcNow;
                            objphone.UpdateDate = DateTime.UtcNow;
                            objphone.SetValue("phoneNumber", phone.PhoneNumber);
                            if (phone.CountryId != 0)
                            {
                                var content = _contentService.GetById((int)phone.CountryId);
                                // Create an Udi of the page
                                var udi = Udi.Create(Constants.UdiEntityType.Document, content.Key);
                                objphone.SetValue("phoneCountry", udi.ToString());
                            }
                            _contentService.SaveAndPublish(objphone);

                        }
                    }
                    return Ok(employee);
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

        #region Delete Employee
        /// <summary>
        /// Delete employee by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id) {
            try
            {

                var toDelete = _contentService.GetById(id);
                if (toDelete.ContentType.Alias == "employee")
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
