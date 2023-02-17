using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ActivityApp.Application.Core.APIController;
using ActivityApp.Application.Core.Exceptions;
using ActivityApp.Application.Implementations;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Employee;
using ActivityApp.Application.Core.ApplicationContracts.Common;

namespace ActivityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : APIBaseController
    {
        private readonly IEmployeeService _EmployeeService;

        public EmployeeController(
            IEmployeeService EmployeeService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<EmployeeController> logger) : base(httpContextAccessor, configuration, logger)
        {
            _EmployeeService = EmployeeService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Getall([FromQuery] GetAllEmployeeRequest request)
        {
            try
            {

                GetAllEmployeeResponse response = await _EmployeeService.GetAllEmployeeAsync(request);

                return base.HandleResponse(response);

            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(errorResult);
            }
        }

        [HttpGet("{Id}")]
        //[SwaggerOperation(Summary = "get exmaple description here")]
        public async Task<IActionResult> Get([FromRoute] GetEmployeeRequest request)
        {
            try
            {
                GetEmployeeResponse response = await _EmployeeService.GetEmployeeAsync(request);

                return base.HandleResponse(response);

            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }

        /// <summary>
        /// exmaple description here
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Employee/Create
        [HttpPost]
        [SwaggerOperation(Summary = "create exmaple description here")]
        public async Task<IActionResult> Post([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                CreateEmployeeResponse response = await _EmployeeService.CreateAsync(request);

                return base.HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }

        /// <summary>
        /// exmaple description here
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Employee/Create
        [HttpPut]
        [SwaggerOperation(Summary = "create exmaple description here")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                UpdateEmployeeResponse response = await _EmployeeService.UpdateAsync(request);

                return base.HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }

        /// <summary>
        /// exmaple description here
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Employee/Delete
        [HttpDelete("{Id}")]
        [SwaggerOperation(Summary = "create exmaple description here")]
        public async Task<IActionResult> Delete([FromRoute] DeleteEmployeeRequest request)
        {
            try
            {
                GenericResponse response = await _EmployeeService.DeleteEmployeeAsync(request);

                return base.HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }


    }
}
