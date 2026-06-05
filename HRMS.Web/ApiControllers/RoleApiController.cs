using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.ApiControllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    public class RoleApiController : ControllerBase
    {
        private readonly IRoleService _service;
        private readonly IIdProtector _idProtector;

        public RoleApiController(
            IRoleService service,
            IIdProtector idProtector)
        {
            _service = service;
            _idProtector = idProtector;
        }

        //  GET ALL 
        //  Only SuperAdmin / Admin
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("GetAllRole")]
        public IActionResult GetAll()
        {
            var roles = _service.GetAll();

            var response = roles.Select(r => new
            {
                RoleId = _idProtector.Protect(r.RoleId.ToString()),
                r.RoleName,
                r.IsActive
            });

            return Ok(response);
        }

        // CREATE
        //  Only SuperAdmin
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("AddRole")]
        public IActionResult Create([FromBody] RoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return BadRequest("Role name is required");

            //  Never trust client-sent IDs
            dto.RoleId = 0;

            _service.Create(dto.RoleName);

            return Ok(new
            {
                message = "Role created successfully"
            });
        }
    }
}
