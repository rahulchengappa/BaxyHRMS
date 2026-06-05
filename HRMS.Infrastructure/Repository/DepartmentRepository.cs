using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace HRMS.Infrastructure.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly string _cs;

        public DepartmentRepository(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection")!;
        }

        //  LIST 
        public List<DepartmentDto> GetAll()
        {
            var list = new List<DepartmentDto>();

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("USP_Department", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "LIST");

            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new DepartmentDto
                {
                    DepartmentId = (int)dr["DepartmentId"],
                    DepartmentName = dr["DepartmentName"].ToString()?? ""
                });
            }

            return list;
        }

        //  INSERT 
        public void Add(DepartmentDto dto)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("USP_Department", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "INSERT");
            cmd.Parameters.AddWithValue("@DepartmentName", dto.DepartmentName);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
