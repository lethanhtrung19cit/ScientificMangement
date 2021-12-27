using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DuAnQLNCKH.Models
{
    public class ReportStatementModel
    {
        private DHTDTTDNEntities1 qLNCKHDHTDTD = null;
        private SqlConnection con;
        public void connection()
        {
            string constr = @"Data Source=DESKTOP-ECMGDNK\SQLEXPRESS;initial catalog=nckh_dhdn;integrated security=True";
            con = new SqlConnection(constr);

        }
        public ReportStatementModel()
        {
            qLNCKHDHTDTD = new DHTDTTDNEntities1();
        }

        public List<Statement> listAll()
        {
            var list = qLNCKHDHTDTD.Database.SqlQuery<Statement>("select * from Statements").ToList();
            return list;
        }
        public bool AddStatement(Statement statement)
        {

            connection();
            SqlCommand com = new SqlCommand("AddStatement", con);
            com.CommandType = CommandType.StoredProcedure;
            
            com.Parameters.AddWithValue("@DateRp", DateTime.Parse(@DateTime.Now.ToString("dd/MM/yyyy")));
            com.Parameters.AddWithValue("@Status", statement.Status);
             
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {

                return true;

            }
            else
            {

                return false;
            }
        }
    }
}