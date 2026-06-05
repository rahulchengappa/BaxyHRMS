
using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using HRMS.Infrastructure.Repository;
using HRMS.Infrastructure.Services;

namespace HRMS.Application.Services
{
    public class AccountService : IAccountService
    {

        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }




        //  SEND RESET LINK
        public bool SendResetLink(string email, string resetUrl)
        {
            var user = _repo.GetEmployeeByEmail(email);
            if (user == null) return false;

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.Now.AddMinutes(30);

            _repo.SavePasswordResetToken(email, token, expiry);

            EmailHelper.SendResetEmail(email, resetUrl + token);
            return true;
        }

        //  RESET PASSWORD
        public bool ResetPassword(string token, string newPassword)
        {
            if (!_repo.ValidateResetToken(token))
                return false;

            var email = _repo.GetEmailFromToken(token);
            if (email == null) return false;

            return _repo.ResetPassword(email, newPassword);
        }

        public LoginResponseDto? Login(LoginRequestDto request)
        {
            var user = _repo.Login(request.Username, request.Password);

            if (user == null)
                return null;

            return new LoginResponseDto
            {
                EmployeeId = user.EmployeeID,
                EmployeeName = $"{user.FirstName} {user.LastName}",
                RoleId = user.RoleId,                 //  REQUIRED
                RoleName = user.RoleName              //  REQUIRED
            };
        }


    }
}
