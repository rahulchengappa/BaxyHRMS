using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace HRMS.Infrastructure.Repository
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly string _cs;

        public DesignationRepository(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection")!;
        }

        //  LIST 
        public List<DesignationDto> GetAll()
        {
            var list = new List<DesignationDto>();

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("USP_Designation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "LIST");

            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new DesignationDto
                {
                    DesignationId = (int)dr["DesignationId"],
                    DesignationName = dr["DesignationName"].ToString() ?? ""
                });
            }

            return list;
        }

        //  INSERT
        public void Add(DesignationDto dto)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("USP_Designation", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", "INSERT");
            cmd.Parameters.AddWithValue("@DesignationName", dto.DesignationName);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
