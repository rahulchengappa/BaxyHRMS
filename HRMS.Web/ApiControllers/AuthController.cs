using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.ApiControllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly JwtTokenService _jwtService;
        private readonly IIdProtector _idProtector;

        public AuthController(
            IAccountService accountService,
            JwtTokenService jwtService,
            IIdProtector idProtector)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _idProtector = idProtector;
        }

        // ================= LOGIN =================
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //  Normalize inputs
            request.Username = request.Username?.Trim() ?? string.Empty;
            request.Password = request.Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required");
            }

            var user = _accountService.Login(request);

            if (user == null || user.EmployeeId <= 0 || string.IsNullOrEmpty(user.RoleName))
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            //  JWT still uses RAW IDs internally 
            var token = _jwtService.GenerateToken(
                user.EmployeeId,
                user.EmployeeName ?? string.Empty,
                user.RoleId,
                user.RoleName
            );

            //  API response uses ENCRYPTED IDs
            return Ok(new
            {
                token,
                user = new
                {
                    EmployeeId = _idProtector.Protect(user.EmployeeId.ToString()),
                    EmployeeName = user.EmployeeName,
                    RoleId = _idProtector.Protect(user.RoleId.ToString()),
                    RoleName = user.RoleName
                }
            });
        }
    }
}
