using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping_system
{
    internal class DataBase
    {
        SqlConnection con = new SqlConnection(@"Data source=DESKTOP-FJFCP05;Initial Catalog=book_peering;Integrated Security=True");

        public void OpenConnection()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void CloseConnection()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return con;
        }
    }
}
