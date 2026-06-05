using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRMS.Application.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(
            int employeeId,
            string employeeName,
            int roleId,
            string roleName)
        {
            var claims = new List<Claim>
            {
                //  ASP.NET CORE STANDARD CLAIMS
                new Claim(ClaimTypes.NameIdentifier, employeeId.ToString()),
                new Claim(ClaimTypes.Name, employeeName),
                new Claim(ClaimTypes.Role, roleName),

                //  MVC / RBAC SUPPORT
                new Claim("EmployeeId", employeeId.ToString()),
                new Claim("EmployeeName", employeeName),
                new Claim("RoleId", roleId.ToString()),
                new Claim("RoleName", roleName),

                //  JWT STANDARD
                new Claim(JwtRegisteredClaimNames.Sub, employeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, employeeName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
