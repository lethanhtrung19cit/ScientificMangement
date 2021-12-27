using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DuAnQLNCKH.Models
{
    public class UserModel
    {
        private SqlConnection con;
        DHTDTTDNEntities1 dHTDTTDNEntities1 = new DHTDTTDNEntities1();
        public void connection()
        {
            string constr = @"Data Source=DESKTOP-ECMGDNK\SQLEXPRESS;initial catalog=nckh_dhdn;integrated security=True";
            con = new SqlConnection(constr);
        }
        public bool AddAccount(Account account)
        {

            connection();

            SqlCommand com = new SqlCommand("AddAccount", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@UserName", account.UserName);
            com.Parameters.AddWithValue("@PassWord", account.PassWord);
            com.Parameters.AddWithValue("@Access", account.Access);
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