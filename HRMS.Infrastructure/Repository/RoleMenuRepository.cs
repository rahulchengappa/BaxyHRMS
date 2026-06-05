using HRMS.Domain.Constants;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HRMS.Infrastructure.Repository
{
    public class RoleMenuRepository : IRoleMenuRepository
    {
        private readonly string _connectionString;

        public RoleMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string missing");
        }

        //  ROLES
        public IEnumerable<RoleDto> GetRoles()
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
                    RoleName = reader["RoleName"]?.ToString() ?? ""
                });
            }

            return roles;
        }

        //  MENUS 
        public IEnumerable<MenuDto> GetMenus()
        {
            var menus = new List<MenuDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Menu", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", MenuSpType.LIST);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                menus.Add(new MenuDto
                {
                    MenuId = Convert.ToInt32(reader["MenuId"]),
                    MenuName = reader["MenuName"]?.ToString() ?? "",
                    MenuUrl = reader["MenuUrl"]?.ToString(),
                    IconClass = reader["IconClass"]?.ToString(),
                    MenuGroup = reader["MenuGroup"]?.ToString() ?? "",
                    ParentMenuId = reader["ParentMenuId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["ParentMenuId"]),
                    MenuOrder = reader["MenuOrder"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MenuOrder"])
                });
            }

            return menus;
        }

        //  MENUS BY ROLE 
        public IEnumerable<MenuDto> GetMenusByRole(int roleId)
        {
            var menus = new List<MenuDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Menu", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", MenuSpType.GET_BY_ROLE);
            cmd.Parameters.AddWithValue("@RoleId", roleId);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                menus.Add(new MenuDto
                {
                    MenuId = Convert.ToInt32(reader["MenuId"]),
                    MenuName = reader["MenuName"]?.ToString() ?? "",
                    MenuUrl = reader["MenuUrl"]?.ToString(),
                    IconClass = reader["IconClass"]?.ToString(),
                    MenuGroup = reader["MenuGroup"]?.ToString() ?? "",
                    ParentMenuId = reader["ParentMenuId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["ParentMenuId"]),
                    MenuOrder = reader["MenuOrder"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MenuOrder"])
                });
            }

            return menus;
        }

        // ================= ASSIGNED MENU IDS 
        public IEnumerable<int> GetAssignedMenuIds(int roleId)
        {
            var menuIds = new List<int>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_RoleMenu", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", RoleMenuSpType.GET_BY_ROLE);
            cmd.Parameters.AddWithValue("@RoleId", roleId);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                menuIds.Add(Convert.ToInt32(reader["MenuId"]));

            return menuIds;
        }

        // ================= SAVE ROLE MENU MAPPING 
        public void SaveRoleMenuMapping(int roleId, IEnumerable<int> menuIds)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            // DELETE OLD
            using (var deleteCmd = new SqlCommand("USP_RoleMenu", con))
            {
                deleteCmd.CommandType = CommandType.StoredProcedure;
                deleteCmd.Parameters.AddWithValue("@Type", RoleMenuSpType.DELETE_BY_ROLE);
                deleteCmd.Parameters.AddWithValue("@RoleId", roleId);
                deleteCmd.ExecuteNonQuery();
            }

            // INSERT NEW
            foreach (var menuId in menuIds)
            {
                using var insertCmd = new SqlCommand("USP_RoleMenu", con);
                insertCmd.CommandType = CommandType.StoredProcedure;

                insertCmd.Parameters.AddWithValue("@Type", RoleMenuSpType.INSERT);
                insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                insertCmd.Parameters.AddWithValue("@MenuId", menuId);

                insertCmd.ExecuteNonQuery();
            }
        }
    }
}
