using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.ApiControllers
{
    [ApiController]
    [Route("api/designations")]
    [Authorize]
    public class DesignationApiController : ControllerBase
    {
        private readonly IDesignationService _service;
        private readonly IIdProtector _idProtector;

        public DesignationApiController(
            IDesignationService service,
            IIdProtector idProtector)
        {
            _service = service;
            _idProtector = idProtector;
        }

        //  GET ALL
        //  Only Admin / SuperAdmin / HR
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        [HttpGet("GetAllDesignation")]
        public IActionResult GetAll()
        {
            var designations = _service.GetAll();

            var response = designations.Select(d => new
            {
                DesignationId = _idProtector.Protect(d.DesignationId.ToString()),
                d.DesignationName
            });

            return Ok(response);
        }

        //  CREATE 
        //  Only SuperAdmin / Admin
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("AddDesignation")]
        public IActionResult Create([FromBody] DesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //  Never trust client-sent IDs
            dto.DesignationId = 0;

            _service.Add(dto);

            return Ok(new
            {
                message = "Designation created successfully"
            });
        }
    }
}
