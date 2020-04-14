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
        public static List<User> login(string email, string password)
        {
            List<User> ret = new List<User>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, UserName FROM Users WHERE Email = @email AND PasswordHash = HASHBYTES('SHA1', CONVERT(NVARCHAR(40), @password)", connection))
                {
                    command.Parameters.Add(new SqlParameter("@email", email));
                    command.Parameters.Add(new SqlParameter("@password", password));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string userName = reader.GetString(1);
                        ret.Add(new User(id, userName));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
        public static List<User> getUsers (string searchTerm)
        {
            List<User> ret = new List<User>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, UserName FROM Users WHERE UserName LIKE '%' + @searchTerm + '%'", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string userName = reader.GetString(1);
                        ret.Add(new User(id, userName));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }

        public static List<Item> getItems(string searchTerm)
        {
            List<Item> ret = new List<Item>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name FROM Items WHERE Name LIKE '%' + @searchTerm + '%'", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));
                    
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        ret.Add(new Item(id, name));
                    }
                    reader.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
        public static List<Item> getItemById(int searchId)
        {
            List<Item> ret = new List<Item>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name FROM Items WHERE Id = @searchId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchId", searchId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        ret.Add(new Item(id, name));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
        public static List<Item> getItemsByCategoryId(int searchCategoryId)
        {
            List<Item> ret = new List<Item>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name FROM Items WHERE CategoryId = @searchCategoryId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchCategoryId", searchCategoryId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        ret.Add(new Item(id, name));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
        public static List<Category> getCategories(string searchTerm)
        {
            List<Category> ret = new List<Category>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name FROM Categories WHERE Name LIKE '%' + @searchTerm + '%'", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        ret.Add(new Category(id, name));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
        public static List<Category> getCategoryById(int searchId)
        {
            List<Category> ret = new List<Category>();

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, Name, Description FROM Categories WHERE Id = @searchId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchId", searchId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string description = reader.GetString(2);
                        ret.Add(new Category(id, name, description));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ret;
        }
    }
}
