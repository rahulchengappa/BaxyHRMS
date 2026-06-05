using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HRMS.Infrastructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString = string.Empty;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<EmployeeDto> GetAllEmployees()
        {
            var list = new List<EmployeeDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "LIST");

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new EmployeeDto
                {
                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                    EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    EmployeeName = reader["EmployeeName"]?.ToString() ?? "",
                    Email = reader["Email"]?.ToString() ?? "",
                    MobileNumber = reader["MobileNumber"]?.ToString() ?? "",
                    DepartmentId = reader["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DepartmentId"]),
                    DepartmentName = reader["DepartmentName"]?.ToString(),
                    DesignationId = reader["DesignationId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DesignationId"]),
                    DesignationName = reader["DesignationName"]?.ToString(),
                    RoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"]?.ToString() ?? "",
                    JoiningDate = reader["JoiningDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["JoiningDate"]),
                    Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                    ReportingManagerId = reader["ReportingManagerId"] == DBNull.Value ? null : Convert.ToInt32(reader["ReportingManagerId"])
                });
            }

            return list;
        }

        public EmployeeDto? GetEmployee(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@EmployeeID", id);

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new EmployeeDto
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                FirstName = reader["FirstName"]?.ToString() ?? "",
                LastName = reader["LastName"]?.ToString() ?? "",
                EmployeeName = reader["EmployeeName"]?.ToString() ?? "",
                Email = reader["Email"]?.ToString() ?? "",
                MobileNumber = reader["MobileNumber"]?.ToString() ?? "",

                DepartmentId = reader["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DepartmentId"]),
                DepartmentName = reader["DepartmentName"]?.ToString(),

                DesignationId = reader["DesignationId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DesignationId"]),
                DesignationName = reader["DesignationName"]?.ToString(),

                RoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]),
                RoleName = reader["RoleName"]?.ToString() ?? "",

                JoiningDate = reader["JoiningDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["JoiningDate"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                ReportingManagerId = reader["ReportingManagerId"] == DBNull.Value ? null : Convert.ToInt32(reader["ReportingManagerId"]),

                CreatedBy = reader["CreatedBy"]?.ToString(),
                UpdatedBy = reader["UpdatedBy"]?.ToString()
            };
        }

        // ✅ FIXED
        public void CreateEmployee(EmployeeDto employee)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "INSERT");
            cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@Email", employee.Email);
            cmd.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);
            cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
            cmd.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
            cmd.Parameters.AddWithValue("@RoleId", employee.RoleId);
            cmd.Parameters.AddWithValue("@JoiningDate", employee.JoiningDate);
            cmd.Parameters.AddWithValue("@Status", employee.Status);
            cmd.Parameters.AddWithValue("@Username", employee.Username);
            cmd.Parameters.AddWithValue("@Password", employee.Password);
            cmd.Parameters.AddWithValue("@CreatedBy", employee.CreatedBy ?? "");
            cmd.Parameters.AddWithValue("@ReportingManagerId",
                employee.ReportingManagerId == null ? DBNull.Value : employee.ReportingManagerId);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var message = ex.Message.ToLower();

                if (message.Contains("exists"))
                {
                    throw new InvalidOperationException(ex.Message);
                }

                throw new InvalidOperationException("Error creating employee: " + ex.Message);
            }
        }

        // ✅ FIXED (MAIN ISSUE)
        public void UpdateEmployee(EmployeeDto employee)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "UPDATE");
            cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
            cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@Email", employee.Email);
            cmd.Parameters.AddWithValue("@MobileNumber", employee.MobileNumber);
            cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
            cmd.Parameters.AddWithValue("@DesignationId", employee.DesignationId);
            cmd.Parameters.AddWithValue("@RoleId", employee.RoleId);
            cmd.Parameters.AddWithValue("@JoiningDate", employee.JoiningDate);
            cmd.Parameters.AddWithValue("@Status", employee.Status);
            cmd.Parameters.AddWithValue("@UpdatedBy", employee.UpdatedBy ?? "");
            cmd.Parameters.AddWithValue("@ReportingManagerId",
                employee.ReportingManagerId == null ? DBNull.Value : employee.ReportingManagerId);

            try
            {
                con.Open();


                var rows = cmd.ExecuteNonQuery();

                // ✅ Do NOT treat 0 rows as error
                // Only SQL exceptions should be treated as failure
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

           
            }
        

        public void DeleteEmployee(int EmployeeId, string UpdatedBy)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "DELETE");
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeId);
            cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateStatus(int employeeId, bool status)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "UPDATE_STATUS");
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@Status", status);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public List<EmployeeDocumentDto> GetEmployeeDocuments(int employeeId)
        {
            var list = new List<EmployeeDocumentDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_EmployeeDocuments", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "GET");
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new EmployeeDocumentDto
                {
                    DocumentId = Convert.ToInt32(reader["DocumentId"]),
                    DocumentType = reader["DocumentType"]?.ToString() ?? "",
                    FileName = reader["FileName"]?.ToString() ?? "",
                    FilePath = reader["FilePath"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public void UploadEmployeeDocument(int employeeId, string documentType, string fileName, string filePath)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_EmployeeDocuments", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "INSERT");
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            cmd.Parameters.AddWithValue("@DocumentType", documentType);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@FilePath", filePath);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteEmployeeDocument(int documentId, int employeeId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_EmployeeDocuments", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "DELETE");
            cmd.Parameters.AddWithValue("@DocumentId", documentId);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public DashboardDto GetDashboardData()
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "DASHBOARD");

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return new DashboardDto();

            return new DashboardDto
            {
                TotalEmployees = reader["TotalEmployees"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalEmployees"]),
                NewJoiners = reader["NewJoiners"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NewJoiners"])
            };
        }

        public void InsertAudit(EmployeeAuditDto audit)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_EmployeeAudit_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeID", audit.EmployeeID);
            cmd.Parameters.AddWithValue("@FieldName", audit.FieldName ?? "");
            cmd.Parameters.AddWithValue("@OldValue", audit.OldValue ?? "");
            cmd.Parameters.AddWithValue("@NewValue", audit.NewValue ?? "");
            cmd.Parameters.AddWithValue("@UpdatedBy", audit.UpdatedBy ?? "");

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public List<EmployeeDto> Filter(string filterBy, string filterValue)
        {
            var list = new List<EmployeeDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "FILTER");
            cmd.Parameters.AddWithValue("@FilterBy", filterBy);
            cmd.Parameters.AddWithValue("@FilterValue", filterValue);

            con.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new EmployeeDto
                {
                    EmployeeCode = reader["EmployeeCode"]?.ToString() ?? "",
                    EmployeeName = reader["EmployeeName"]?.ToString() ?? "",
                    Email = reader["Email"]?.ToString() ?? "",
                    MobileNumber = reader["MobileNumber"]?.ToString() ?? "",
                    DepartmentName = reader["DepartmentName"]?.ToString(),
                    DesignationName = reader["DesignationName"]?.ToString(),
                    RoleName = reader["RoleName"]?.ToString() ?? "",
                    JoiningDate = reader["JoiningDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["JoiningDate"]),
                    Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"])
                });
            }

            return list;
        }

        public EmployeeDto? Login(string username, string password)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "LOGIN");
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new EmployeeDto
            {
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                FirstName = reader["FirstName"]?.ToString() ?? "",
                LastName = reader["LastName"]?.ToString() ?? "",
                Email = reader["Email"]?.ToString() ?? "",
                Username = reader["Username"]?.ToString() ?? "",
                RoleId = reader["RoleId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RoleId"]),
                RoleName = reader["RoleName"]?.ToString() ?? ""
            };
        }
    }
}