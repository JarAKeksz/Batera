using System;
using System.CodeDom;
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
        public static User logIn(string email, string password)
        {
            User ret = null;

            try
            {
                using (SqlCommand command = new SqlCommand("SELECT Id, UserName FROM Users WHERE Email = @email AND PasswordHash = HASHBYTES('SHA1', CONVERT(NVARCHAR(40), @password))", connection))
                {
                    command.Parameters.Add(new SqlParameter("@email", email));
                    command.Parameters.Add(new SqlParameter("@password", password));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string userName = reader.GetString(1);
                        ret = new User(id, userName);
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
        public static User signUp(string userName, string name, string email, string password)
        {
            try
            {
                bool emailIsTaken = false;
                using (SqlCommand command = new SqlCommand("CASE WHEN EXISTS(SELECT Email FROM Users WHERE Email = @email) THEN 1 ELSE 0 END", connection))
                {
                    command.Parameters.Add(new SqlParameter("@email", email));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        emailIsTaken = reader.GetInt32(0) == 1;
                    }
                    reader.Close();
                }
                if(emailIsTaken)
                    Console.WriteLine("Az email foglalt.");

                bool nameIsTaken = false;
                using (SqlCommand command = new SqlCommand("CASE WHEN EXISTS(SELECT UserName FROM Users WHERE UserName = @userName) THEN 1 ELSE 0 END", connection))
                {
                    command.Parameters.Add(new SqlParameter("@userName", userName));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        nameIsTaken = reader.GetInt32(0) == 1;
                    }
                    reader.Close();
                }
                if (nameIsTaken)
                    Console.WriteLine("A felhasznalonev foglalt.");

                if (!emailIsTaken && !nameIsTaken)
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO Users (UserName, Name, Email, PasswordHash) VALUES(@userName, @name, @email, HASHBYTES('SHA1', CONVERT(NVARCHAR(40), @password)))", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userName", userName));
                        command.Parameters.Add(new SqlParameter("@name", name));
                        command.Parameters.Add(new SqlParameter("@email", email));
                        command.Parameters.Add(new SqlParameter("@password", password));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return logIn(email, password);
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
        public static Item getItemById(int searchId)
        {
            Item ret = null;

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
                        ret = new Item(id, name);
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
