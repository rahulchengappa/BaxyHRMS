using HRMS.Domain.Constants;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HRMS.Infrastructure.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string missing");
        }

        // ================= LIST =================
        public IEnumerable<RoleDto> GetAll()
        {
            var roles = new List<RoleDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Role", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", RoleSpType.LIST);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(new RoleDto
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"]?.ToString() ?? "",
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return roles;
        }

        // ================= INSERT =================
        public void Insert(string roleName)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Role", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", RoleSpType.INSERT);
            cmd.Parameters.AddWithValue("@RoleName", roleName);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        // EXISTS 
       
        public bool RoleExists(string roleName)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM RoleMaster WHERE RoleName = @RoleName",
                con);

            cmd.Parameters.AddWithValue("@RoleName", roleName);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
