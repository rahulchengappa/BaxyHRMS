using System.Data;
using Microsoft.Data.SqlClient;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using HRMS.Domain.Constants;
using Microsoft.Extensions.Configuration;

namespace HRMS.Infrastructure.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string not configured");
        }

        //  LOGIN 
        public AuthEmployeeDto? Login(string username, string password)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Type", SqlDbType.VarChar, 50).Value = "LOGIN";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new AuthEmployeeDto
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                RoleId = Convert.ToInt32(reader["RoleId"]),
                RoleName = reader["RoleName"]?.ToString()
            };
        }

        //  PROFILE 
        public EmployeeDto? GetEmployeeById(int employeeId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", EmployeeSpType.GET_BY_ID);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new EmployeeDto
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty ,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Username = reader["Username"]?.ToString() ?? string.Empty
            };
        }

        //  GET BY EMAIL 
        public EmployeeDto? GetEmployeeByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", EmployeeSpType.GET_BY_EMAIL);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new EmployeeDto
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                Email = reader["Email"]?.ToString() ?? string.Empty,
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                Username = reader["Username"]?.ToString() ?? string.Empty
            };
        }

        // PASSWORD RESET 
        public bool SavePasswordResetToken(string email, string token, DateTime expiry)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_SavePasswordResetToken", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@Expiry", expiry);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ValidateResetToken(string token)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_ValidateResetToken", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Token", token);

            conn.Open();
            var result = cmd.ExecuteScalar();

            return result != null && Convert.ToInt32(result) == 1;
        }

        public string? GetEmailFromToken(string token)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_GetEmailFromToken", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Token", token);

            conn.Open();
            return cmd.ExecuteScalar()?.ToString();
        }

        public bool ResetPassword(string email, string newPassword)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_ResetPassword", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", newPassword);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
