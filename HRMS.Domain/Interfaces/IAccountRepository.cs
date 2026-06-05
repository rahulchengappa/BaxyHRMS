using HRMS.Domain.Entities;

namespace HRMS.Domain.Interfaces
{
    public interface IAccountRepository
    {
        AuthEmployeeDto? Login(string username, string password);
        EmployeeDto? GetEmployeeByEmail(string email);

        bool SavePasswordResetToken(string email, string token, DateTime expiry);

        bool ValidateResetToken(string token);

        string? GetEmailFromToken(string token);

        bool ResetPassword(string email, string newPassword);

        EmployeeDto? GetEmployeeById(int employeeId);

    }
}
