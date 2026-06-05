using HRMS.Application.DTOs.Auth;
using HRMS.Domain.Entities;

namespace HRMS.Application.Interfaces
{
    public interface IAccountService
    {


        LoginResponseDto? Login(LoginRequestDto request);//  PASSWORD RESET
        bool SendResetLink(string email, string resetUrl);
        bool ResetPassword(string token, string newPassword);
    }
}
