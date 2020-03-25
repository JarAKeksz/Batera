using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DataBase
    {
        static private SqlConnection connection;

        public static bool connect()
        {
            connection = new SqlConnection(Properties.Settings.Default.BateraDBConnectionString);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Item> getItems(string searchTerm)
        {
            List<Item> ret = new List<Item>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name, Price FROM Items WHERE Name LIKE '%@searchTerm%'", connection))
                {
                    command.Parameters.Add(new SqlParameter("searchTerm", searchTerm));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        ret.Add(new Item(id, name));
                    }
                }
            }
            catch
            {
                Console.WriteLine("Exception");
            }

            return ret;
        }
    }
}
