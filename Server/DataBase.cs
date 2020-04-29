using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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

        public static User logIn(string token, string email = "", string password = "")
        {
            User ret = null;

            try
            {
                if (token != null)
                {
                    string tokenCheckQuery = "SELECT Id, UserName FROM Users WHERE Token = @logInToken";
                    using (SqlCommand command = new SqlCommand(tokenCheckQuery, connection))
                    {
                        command.Parameters.Add("@logInToken", SqlDbType.Char).Value = token;

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string userName = reader.GetString(1);
                            string logInToken = token;
                            ret = new User(id, userName, logInToken);
                        }
                        else
                        {
                            ret = null;
                        }
                        reader.Close();
                    }
                }
                else
                {
                    string query = "SELECT Id, UserName, CONVERT(NVARCHAR(64) , HASHBYTES('SHA2_256', CONCAT(Id, UserName, GETDATE())), 2) FROM Users " +
                        "WHERE Email = @email AND PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@email", email));
                        //command.Parameters.Add(new SqlParameter("@password", System.Data.SqlDbType.VarChar).Value = password);
                        command.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string userName = reader.GetString(1);
                            string logInToken = reader.GetString(2);
                            ret = new User(id, userName, logInToken);
                        }
                        else
                        {
                            ret = null;
                        }
                        reader.Close();
                    }

                    if (ret != null)
                    {
                        string createTokenQuery = "UPDATE Users SET Token = @logInToken WHERE Id = @id";
                        using (SqlCommand update = new SqlCommand(createTokenQuery, connection))
                        {
                            //update.Parameters.Add(new SqlParameter("@logInToken", ret.logInToken));
                            update.Parameters.Add("@logInToken", SqlDbType.Char).Value = ret.logInToken;
                            update.Parameters.Add(new SqlParameter("@id", ret.id));

                            Console.WriteLine("Rows affected: " + update.ExecuteNonQuery());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ret = null;
            }

            return ret;
        }

        public static User signUp(string userName, string name, string email, string password, string birthDate, string phone = null)
        {
            try
            {
                bool emailIsTaken = false;
                string emailCheckQuery = "CASE WHEN EXISTS(SELECT Email FROM Users WHERE Email = @email) THEN 1 ELSE 0 END";
                using (SqlCommand command = new SqlCommand(emailCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@email", email));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        emailIsTaken = reader.GetInt32(0) == 1;
                    }
                    reader.Close();
                }
                if(emailIsTaken)
                    Console.WriteLine("Az email foglalt.");

                bool nameIsTaken = false;
                string nameCheckQuery = "CASE WHEN EXISTS(SELECT UserName FROM Users WHERE UserName = @userName) THEN 1 ELSE 0 END";
                using (SqlCommand command = new SqlCommand(nameCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userName", userName));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        nameIsTaken = reader.GetInt32(0) == 1;
                    }
                    reader.Close();
                }
                if (nameIsTaken)
                    Console.WriteLine("A felhasznalonev foglalt.");

                if (!emailIsTaken && !nameIsTaken)
                {
                    string query = "INSERT INTO Users (UserName, Name, Email, PasswordHash, BirthDate) " +
                        "VALUES(@userName, @name, @email, CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2), @birthDate)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userName", userName));
                        command.Parameters.Add(new SqlParameter("@name", name));
                        command.Parameters.Add(new SqlParameter("@email", email));
                        command.Parameters.Add(new SqlParameter("@password", password));
                        command.Parameters.Add(new SqlParameter("@birthDate", birthDate));

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

        public static List<User> getUsers (Dictionary<string, string> searchTerms = null)
        {
            List<User> ret = new List<User>();

            try
            {
                string query = "SELECT Id, UserName FROM Users";
                if (searchTerms != null)
                {
                    query += " WHERE ";
                    foreach (string key in searchTerms.Keys)
                    {
                        switch (key)
                        {
                            case "Name":
                            case "UserName":
                            case "Email":
                            case "Phone":
                                query += key + " LIKE '%' + @" + key + " + '%'";
                                break;

                            case "Id":
                                query += key + " = @" + key;
                                break;

                            case "MinBirthDate":
                                query += "BirthDate >= '@" + key + "'";
                                break;
                            case "MaxBirthDate":
                                query += "BirthDate <= '@" + key + "'";
                                break;
                        }
                        if (key != searchTerms.Last().Key)
                        {
                            query += " AND ";
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (KeyValuePair<string, string> pair in searchTerms)
                    {
                        string tmp = "@" + pair.Key;
                        command.Parameters.Add(new SqlParameter(tmp, pair.Value));
                    }

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

        public static List<Item> getItems(Dictionary<string, string> searchTerms)
        {
            List<Item> ret = new List<Item>();

            try
            {
                string query = "SELECT i.Id, i.Name, c.Name, i.Price, i.Image FROM Items AS i JOIN Categories AS c ON i.CategoryId = c.Id";
                if (searchTerms != null)
                {
                    query += " WHERE ";
                    foreach (string key in searchTerms.Keys)
                    {
                        query += "i.";
                        switch (key)
                        {
                            case "Name":
                            case "Description":
                                query += key + " LIKE '%' + @" + key + " + '%'";
                                break;

                            case "Id":
                            case "Seller":
                            case "CategoryId":
                            case "TopBidUser":
                            case "IsItNew":
                            case "BuyWithoutBid":
                                query += key + " = @" + key;
                                break;

                            case "MinDate":
                                query += "Date >= '@" + key + "'";
                                break;
                            case "MaxDate":
                                query += "Date <= '@" + key + "'";
                                break;
                            case "MinEndDate":
                                query += "EndDate >= '@" + key + "'";
                                break;
                            case "MaxEndDate":
                                query += "EndDate <= '@" + key + "'";
                                break;
                            case "MinBid":
                                query += "TopBid >= @" + key;
                                break;
                            case "MaxBid":
                                query += "TopBid <= @" + key;
                                break;
                            case "MinPrice":
                                query += "Price >= @" + key;
                                break;
                            case "MaxPrice":
                                query += "Price <= @" + key;
                                break;
                        }
                        if (key != searchTerms.Last().Key)
                        {
                            query += " AND ";
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (KeyValuePair<string, string> pair in searchTerms)
                    {
                        string tmp = "@" + pair.Key;
                        command.Parameters.Add(new SqlParameter(tmp, pair.Value));
                    }
                    
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        int price = reader.GetInt32(3);
                        string image = reader.GetString(4);
                        ret.Add(new Item(id, name, category, price, image));
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
                string query = "SELECT i.Id, i.Name, c.Name, i.Price, i.Image FROM Items AS i JOIN Categories AS c ON i.CategoryId = c.Id " +
                    "WHERE i.Id = @searchId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchId", searchId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        int price = reader.GetInt32(3);
                        string image = reader.GetString(4);
                        ret = new Item(id, name, category, price, image);
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
                string query = "SELECT Id, Name FROM Categories WHERE Name LIKE '%' + @searchTerm + '%'";
                using (SqlCommand command = new SqlCommand(query, connection))
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

        public static Category getCategoryById(int searchId)
        {
            Category ret = null;

            try
            {
                string query = "SELECT Name, Description FROM Categories WHERE Id = @searchId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@searchId", searchId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string description = reader.GetString(0);
                        ret = new Category(searchId, name, description);
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
