using HRMS.Application.DTOs.Employee;
using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeeApiController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IIdProtector _idProtector;

    public EmployeeApiController(
        IEmployeeService service,
        IIdProtector idProtector)
    {
        _service = service;
        _idProtector = idProtector;
    }

    // GET ALL
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("GetAllEmployee")]
    public IActionResult GetAll()
    {
        var employees = _service.GetAll();

        var response = employees.Select(e => new EmployeeResponseDto
        {
            EmployeeID = _idProtector.Protect(e.EmployeeID.ToString()),
            EmployeeCode = e.EmployeeCode ?? string.Empty,
            FullName = $"{e.FirstName ?? ""} {e.LastName ?? ""}".Trim(),
            Email = e.Email ?? string.Empty,
            MobileNumber = e.MobileNumber ?? string.Empty,

            DepartmentId = _idProtector.Protect(e.DepartmentId.ToString()),
            DepartmentName = e.DepartmentName ?? string.Empty,

            DesignationId = _idProtector.Protect(e.DesignationId.ToString()),
            DesignationName = e.DesignationName ?? string.Empty,

            RoleId = _idProtector.Protect(e.RoleId.ToString()),
            RoleName = e.RoleName ?? string.Empty,

            JoiningDate = e.JoiningDate,
            Status = e.Status
        });

        return Ok(response);
    }

    // GET BY ID
    [HttpGet("GetEmployeeById")]
    public IActionResult GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Employee ID is required");

        int employeeId;
        try
        {
            employeeId = int.Parse(_idProtector.Unprotect(id));
        }
        catch
        {
            return BadRequest("Invalid employee ID");
        }

        var loggedInIdStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!int.TryParse(loggedInIdStr, out int loggedInEmployeeId))
            return Unauthorized();

        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        bool canViewAll = role is "SuperAdmin" or "Admin" or "HR";

        if (!canViewAll && employeeId != loggedInEmployeeId)
            return Forbid();

        var e = _service.GetById(employeeId);
        if (e == null)
            return NotFound();

        return Ok(new EmployeeResponseDto
        {
            EmployeeID = _idProtector.Protect(e.EmployeeID.ToString()),
            EmployeeCode = e.EmployeeCode ?? string.Empty,
            FullName = $"{e.FirstName ?? ""} {e.LastName ?? ""}".Trim(),
            Email = e.Email ?? string.Empty,
            MobileNumber = e.MobileNumber ?? string.Empty,

            DepartmentId = _idProtector.Protect(e.DepartmentId.ToString()),
            DepartmentName = e.DepartmentName ?? string.Empty,

            DesignationId = _idProtector.Protect(e.DesignationId.ToString()),
            DesignationName = e.DesignationName ?? string.Empty,

            RoleId = _idProtector.Protect(e.RoleId.ToString()),
            RoleName = e.RoleName ?? string.Empty,

            JoiningDate = e.JoiningDate,
            Status = e.Status
        });
    }

    // CREATE
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("AddEmployee")]
    public IActionResult Create([FromBody] EmployeeCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.DepartmentId <= 0 ||
            request.DesignationId <= 0 ||
            request.RoleId <= 0)
        {
            return BadRequest("Invalid related IDs");
        }

        _service.Create(request, "API");

        return Ok(new { message = "Employee created successfully" });
    }

    // UPDATE
    [HttpPut("UpdateEmployee")]
    public IActionResult Update([FromBody] EmployeeUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.EmployeeID <= 0 ||
            request.DepartmentId <= 0 ||
            request.DesignationId <= 0 ||
            request.RoleId <= 0)
        {
            return BadRequest("Invalid request data");
        }

        int employeeId;
        try
        {
            employeeId = request.EmployeeID;
        }
        catch
        {
            return BadRequest("Invalid EmployeeID");
        }

        var loggedInIdStr = User.FindFirstValue("sub");

        if (!int.TryParse(loggedInIdStr, out int loggedInEmployeeId))
            return Unauthorized();

        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        bool canEditAll = role is "SuperAdmin" or "Admin";

        if (!canEditAll && employeeId != loggedInEmployeeId)
            return Forbid();

        var existing = _service.GetById(employeeId);
        if (existing == null)
            return NotFound("Employee not found");

        var emp = new EmployeeDto
        {
            EmployeeID = existing.EmployeeID,
            FirstName = request.FirstName ?? existing.FirstName,
            LastName = request.LastName ?? existing.LastName,
            Email = request.Email ?? existing.Email,
            MobileNumber = request.MobileNumber ?? existing.MobileNumber,

            DepartmentId = request.DepartmentId,
            DesignationId = request.DesignationId,
            RoleId = canEditAll ? request.RoleId : existing.RoleId,

            JoiningDate = request.JoiningDate,
            Status = request.Status,

            UpdatedBy = "API"
        };

        _service.UpdateEmployee(emp);

        return Ok(new { message = "Employee updated successfully" });
    }

    // DELETE
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("DeleteEmployee")]
    public IActionResult Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Employee ID is required");

        int employeeId;
        try
        {
            employeeId = int.Parse(_idProtector.Unprotect(id));
        }
        catch
        {
            return BadRequest("Invalid employee ID");
        }

        _service.Delete(employeeId, "API");

        return Ok(new { message = "Employee deleted successfully" });
    }
}