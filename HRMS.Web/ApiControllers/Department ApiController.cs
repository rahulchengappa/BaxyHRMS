using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.ApiControllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentApiController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IIdProtector _idProtector;

        public DepartmentApiController(
            IDepartmentService service,
            IIdProtector idProtector)
        {
            _service = service;
            _idProtector = idProtector;
        }

        //  GET ALL 
        //  Only Admin / SuperAdmin / HR
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        [HttpGet("getalldepartment")]
        public IActionResult GetAll()
        {
            var departments = _service.GetAll();

            var response = departments.Select(d => new
            {
                DepartmentId = _idProtector.Protect(d.DepartmentId.ToString()),
                d.DepartmentName
            });

            return Ok(response);
        }

        // CREATE 
        //  Only SuperAdmin / Admin
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("AddDepartment")]
        public IActionResult Create([FromBody] DepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Never trust client-sent IDs
            dto.DepartmentId = 0;

            _service.Add(dto);

            return Ok(new
            {
                message = "Department created successfully"
            });
        }
    }
}
