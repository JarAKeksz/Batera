using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
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

        public static User logIn(string token)
        {
            User ret = null;

            try
            {
                string tokenCheckQuery = "SELECT Id, UserName, Name, BirthDate, Email, Phone FROM Users WHERE Token = @logInToken";
                using (SqlCommand command = new SqlCommand(tokenCheckQuery, connection))
                {
                    command.Parameters.Add("@logInToken", SqlDbType.Char).Value = token;

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string userName = reader.GetString(1);
                        string logInToken = token;
                        string name = reader.GetString(2);
                        string birthDate = reader.GetString(3);
                        string emailAddr = reader.GetString(4);
                        ret = new User(id, userName, logInToken, name, birthDate, emailAddr);
                        if (!reader.IsDBNull(5))
                        {
                            ret.phone = reader.GetString(5);
                        }
                    }
                    else
                    {
                        ret = null;
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ret = null;
            }

            return ret;
        }

        public static User logIn(string email, string password)
        {
            User ret = null;

            try
            {
                string query = "SELECT Id, UserName, CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', CONCAT(Id, UserName, GETDATE())), 2), Name, BirthDate, Email, Phone " +
                    "FROM Users WHERE Email = @email AND PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2)";
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
                        string name = reader.GetString(3);
                        string birthDate = reader.GetString(4);
                        string emailAddr = reader.GetString(5);
                        ret = new User(id, userName, logInToken, name, birthDate, emailAddr);
                        if (!reader.IsDBNull(6))
                        {
                            ret.phone = reader.GetString(6);
                        }
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                ret = null;
            }

            return ret;
        }

        public static byte signUp(string userName, string name, string email, string password, string birthDate, string phone = null)
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
                        emailIsTaken = (bool)reader[0];
                    }
                    reader.Close();
                }
                if (emailIsTaken)
                {
                    Console.WriteLine("Az email foglalt.");
                    return 1; // == foglalt email cím
                }

                bool nameIsTaken = false;
                string nameCheckQuery = "CASE WHEN EXISTS(SELECT UserName FROM Users WHERE UserName = @userName) THEN 1 ELSE 0 END";
                using (SqlCommand command = new SqlCommand(nameCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userName", userName));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        nameIsTaken = (bool)reader[0];
                    }
                    reader.Close();
                }
                if (nameIsTaken)
                {
                    Console.WriteLine("A felhasznalonev foglalt.");
                    return 2; // == foglalt felhasználónév
                }

                if (!emailIsTaken && !nameIsTaken)
                {
                    string query;
                    if (phone != null)
                    {
                        query = "INSERT INTO Users (UserName, Name, Email, PasswordHash, BirthDate, Phone) " +
                            "VALUES(@userName, @name, @email, CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2), @birthDate, @phone)";
                    }
                    else
                    {
                        query = "INSERT INTO Users (UserName, Name, Email, PasswordHash, BirthDate) " +
                            "VALUES(@userName, @name, @email, CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2), @birthDate)";
                    }
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userName", userName));
                        command.Parameters.Add(new SqlParameter("@name", name));
                        command.Parameters.Add(new SqlParameter("@email", email));
                        command.Parameters.Add(new SqlParameter("@password", password));
                        command.Parameters.Add(new SqlParameter("@birthDate", birthDate));
                        if (phone != null)
                        {
                            command.Parameters.Add(new SqlParameter("@phone", phone));
                        }

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 3; // == vmi más baj van
            }
            return 0; // == minden rendben
        }

        public static byte addItem(string name, int categoryId, string image, int sellerId, string description, string date, string endDate,
            bool isItNew, bool buyWithoutBid, int bidStart, int price = -1)
        {
            try
            {
                string query;
                if (buyWithoutBid && price != -1)
                {
                    query = "INSERT INTO Items (Name, Seller, CategoryId, Image, Description, Date, EndDate, IsItNew, BuyWithoutBid, Price, BidStart) " +
                        "VALUES(@Name, @Seller, @CategoryId, @Image, @Description, @Date, @EndDate, @IsItNew, @BuyWithoutBid, @Price, @BidStart)";
                }
                else if (!buyWithoutBid)
                {
                    query = "INSERT INTO Items (Name, Seller, CategoryId, Image, Description, Date, EndDate, IsItNew, BidStart) " +
                        "VALUES(@Name, @Seller, @CategoryId, @Image, @Description, @Date, @EndDate, @IsItNew, @BidStart)";
                }
                else
                {
                    return 1; // == buyWithoutBid igaz, de nem adtál meg árat
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", name));
                    command.Parameters.Add(new SqlParameter("@Seller", sellerId));
                    command.Parameters.Add(new SqlParameter("@CategoryId", categoryId));
                    command.Parameters.Add(new SqlParameter("@Image", image));
                    command.Parameters.Add(new SqlParameter("@Description", description));
                    command.Parameters.Add(new SqlParameter("@Date", date));
                    command.Parameters.Add(new SqlParameter("@EndDate", endDate));
                    command.Parameters.Add(new SqlParameter("@IsItNew", isItNew));
                    command.Parameters.Add(new SqlParameter("@BidStart", bidStart));
                    if (buyWithoutBid && price != -1)
                    {
                        command.Parameters.Add(new SqlParameter("@BidStart", bidStart));
                        command.Parameters.Add(new SqlParameter("@Price", price));
                    }

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                if(e.Message.Contains("DateCheck"))
                {
                    return 2; // == sql date check megszegve
                }
                else
                if (e.Message.Contains("Unique"))
                {
                    return 3; // == sql unique check megszegve
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 4; // == vmi más baj van
            }
            return 0; // == minden rendben
        }

        public static User modifyUser(User user, Dictionary<string, string> changes)
        {
            try
            {
                if (changes != null)
                {
                    string query = "UPDATE Users SET ";
                    foreach (string key in changes.Keys)
                    {
                        if (key == "Name" || key == "UserName" || key == "Email" || key == "Phone")
                        {
                            query += key + " = [@" + key + "]";
                        }
                        else if (key == "BirthDate")
                        {
                            query += key + " = '@" + key + "'";
                        }
                        else if (key == "Password")
                        {
                            query += "PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @Password), 2)";
                        }

                        if (key != changes.Last().Key)
                        {
                            query += ", ";
                        }
                    }
                    query += " WHERE Id = @id";

                    using (SqlCommand update = new SqlCommand(query, connection))
                    {
                        if (changes != null)
                        {
                            foreach (KeyValuePair<string, string> pair in changes)
                            {
                                string tmp = "@" + pair.Key;
                                update.Parameters.Add(new SqlParameter(tmp, pair.Value));

                                switch (pair.Key)
                                {
                                    case "Name":
                                        user.name = pair.Value;
                                        break;
                                    case "UserName":
                                        user.userName = pair.Value;
                                        break;
                                    case "Email":
                                        user.email = pair.Value;
                                        break;
                                    case "Phone":
                                        user.phone = pair.Value;
                                        break;
                                    case "BirthDate":
                                        user.birthDate = pair.Value;
                                        break;
                                }
                            }
                        }

                        update.Parameters.Add(new SqlParameter("@id", user.id));

                        Console.WriteLine("Rows affected: " + update.ExecuteNonQuery());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return user;
        }

        public static List<User> getUsers(Dictionary<string, string> searchTerms = null)
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

                            default:
                                return null;
                        }
                        if (key != searchTerms.Last().Key)
                        {
                            query += " AND ";
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (searchTerms != null)
                    {
                        foreach (KeyValuePair<string, string> pair in searchTerms)
                        {
                            string tmp = "@" + pair.Key;
                            command.Parameters.Add(new SqlParameter(tmp, pair.Value));
                        }
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
                string query = "SELECT i.Id, i.Name, c.Name, ISNULL(i.Price,-1), ISNULL(MAX(b.Value),i.BidStart), i.Image " +
                    "FROM Items AS i JOIN Categories AS c ON i.CategoryId = c.Id LEFT JOIN Bids AS b ON i.Id = b.ItemId";
                bool eMinBid = false;
                bool eMaxBid = false;
                if (searchTerms != null)
                {
                    query += " WHERE ";
                    foreach (string key in searchTerms.Keys)
                    {
                        switch (key)
                        {
                            case "Name":
                            case "Description":
                                query += "i." + key + " LIKE '%' + @" + key + " + '%'";
                                break;

                            case "Id":
                            case "Seller":
                            case "CategoryId":
                            case "TopBidUser":
                            case "IsItNew":
                            case "BuyWithoutBid":
                                query += "i." + key + " = @" + key;
                                break;

                            /*
                            case "CategoryId":
                                query += "i.CategoryId IN(@CategoryId)";
                                break;*/

                            case "MinDate":
                                query += "i.Date >= '@" + key + "'";
                                break;
                            case "MaxDate":
                                query += "i.Date <= '@" + key + "'";
                                break;
                            case "MinEndDate":
                                query += "i.EndDate >= '@" + key + "'";
                                break;
                            case "MaxEndDate":
                                query += "i.EndDate <= '@" + key + "'";
                                break;
                            case "MinPrice":
                                query += "i.Price >= @" + key;
                                break;
                            case "MaxPrice":
                                query += "i.Price <= @" + key;
                                break;

                            case "MinBid":
                                eMinBid = true;
                                break;
                            case "MaxBid":
                                eMaxBid = true;
                                break;

                            default:
                                return null;
                        }
                        if (key != searchTerms.Last().Key)
                        {
                            query += " AND ";
                        }
                    }
                }
                query += " GROUP BY i.Id, i.Name, c.Name, i.Price, i.BidStart, i.Image";
                if (eMinBid && eMaxBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) >= @MinBid AND ISNULL(MAX(b.Value),i.BidStart) <= @MaxBid";
                }
                else if (eMinBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) >= @MinBid";
                }
                else if (eMaxBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) <= @MaxBid";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (searchTerms != null)
                    {
                        foreach (KeyValuePair<string, string> pair in searchTerms)
                        {
                            string tmp = "@" + pair.Key;
                            command.Parameters.Add(new SqlParameter(tmp, pair.Value));
                        }
                    }

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        int price = reader.GetInt32(3);
                        int current = reader.GetInt32(4);
                        string image = reader.GetString(5);

                        ret.Add(new Item(id, name, category, price, current, image));
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

        public static DetailedItem getItemDetails(int itemId)
        {
            DetailedItem ret = null;

            try
            {
                string query = "SELECT i.Name, c.Name, ISNULL(i.Price,-1), ISNULL(MAX(b.Value),i.BidStart), i.Image, u.UserName, i.Description, i.Date, i.EndDate, " +
                    "i.IsItNew, i.BuyWithoutBid, i.BidStart " +
                    "FROM Items AS i JOIN Categories AS c ON i.CategoryId = c.Id LEFT JOIN Bids AS b ON i.Id = b.ItemId LEFT JOIN Users AS u ON i.Seller = u.Id " +
                    "WHERE i.Id = @itemId " +
                    "GROUP BY i.Name, c.Name, i.Price, i.BidStart, i.Image, u.UserName, i.Description, i.Date, i.EndDate, i.IsItNew, i.BuyWithoutBid, i.BidStart";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int id = itemId;
                        string name = reader.GetString(0);
                        string category = reader.GetString(1);
                        int price = reader.GetInt32(2);
                        int current = reader.GetInt32(3);
                        string image = reader.GetString(4);

                        string seller = reader.GetString(5);
                        string description = reader.GetString(6);
                        string date = reader.GetString(7);
                        string endDate = reader.GetString(8);
                        bool isItNew = (bool)reader[9];
                        bool buyWithoutBid = (bool)reader[10];
                        int bidStart = reader.GetInt32(11);

                        List<Bid> tmp = new List<Bid>();

                        string bidsQuery = "SELECT u.Id, u.UserName, b.Value FROM Bids AS b JOIN Users AS u ON b.UserId = u.Id WHERE b.ItemId = @itemId";
                        using (SqlCommand subcommand = new SqlCommand(bidsQuery, connection))
                        {
                            subcommand.Parameters.Add(new SqlParameter("@itemId", itemId));

                            SqlDataReader bidReader = subcommand.ExecuteReader();
                            if (bidReader.Read())
                            {
                                int userId = bidReader.GetInt32(0);
                                string userName = bidReader.GetString(1);
                                int value = bidReader.GetInt32(2);

                                tmp.Add(new Bid(userId, userName, value));
                            }
                            bidReader.Close();
                        }

                        ret = new DetailedItem(id, name, category, price, current, image, seller, description, date, endDate, isItNew, buyWithoutBid, bidStart, tmp);
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

        public static List<Category> getCategories(bool description = false, string searchTerm = null)
        {
            List<Category> ret = new List<Category>();

            try
            {
                string query = "SELECT Id, Name";
                if (description)
                {
                    query += ", Description";
                }
                query += " FROM Categories";
                if (searchTerm != null)
                {
                    query += " WHERE Name LIKE '%' + @searchTerm + '%' OR Description LIKE '%' + @searchTerm + '%'";
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (searchTerm != null)
                    {
                        command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));
                    }

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        if (description)
                        {
                            string desc = reader.GetString(2);
                            ret.Add(new Category(id, name, desc));
                        }
                        else
                        {
                            ret.Add(new Category(id, name));
                        }
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

        public static List<Item> getFavorites(string userId)
        {
            List<Item> ret = new List<Item>();

            try
            {
                string query = "SELECT f.ItemId, i.Name, c.Name, ISNULL(i.Price,-1), ISNULL(MAX(b.Value),i.BidStart), i.Image " +
                    "FROM Favorites AS f JOIN Items AS i ON f.ItemId = i.Id JOIN Categories AS c ON i.CategoryId = c.Id " +
                    "LEFT JOIN Bids AS b ON i.Id = b.ItemId WHERE f.UserId = @userId GROUP BY f.ItemId, i.Name, c.Name, i.Price, i.BidStart, i.Image";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userId", userId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        int price = reader.GetInt32(3);
                        int current = reader.GetInt32(4);
                        string image = reader.GetString(5);

                        ret.Add(new Item(id, name, category, price, current, image));
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

        public static byte toggleFavorite(string userId, int itemId)
        {
            try
            {
                bool favoriteToggle = false;
                string favoriteCheckQuery = "CASE WHEN EXISTS(SELECT ItemId FROM Favorites WHERE ItemId = @itemId AND UserId = @userId) THEN 1 ELSE 0 END";
                using (SqlCommand command = new SqlCommand(favoriteCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userId", userId));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        favoriteToggle = (bool)reader[0];
                    }
                    reader.Close();
                }
                if(favoriteToggle)
                {
                    string query = "DELETE Favorites WHERE ItemId = @itemId AND UserId = @userId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userId", userId));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    return 0;
                }
                else
                {
                    string query = "INSERT INTO Favorites (ItemId, UserId) VALUES(@itemId, @userId)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userId", userId));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 2;
            }
        }

        public static List<Item> getBidsByUser(string userId)
        {
            List<Item> ret = new List<Item>();

            try
            {
                string query = "SELECT b.ItemId, i.Name, c.Name, ISNULL(i.Price,-1), b.Value, i.Image " +
                    "FROM Bids AS b JOIN Items AS i ON b.ItemId = i.Id JOIN Categories AS c ON i.CategoryId = c.Id WHERE b.userId = @userId " +
                    "GROUP BY b.ItemId, i.Name, c.Name, i.Price, b.Value, i.BidStart, i.Image";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userId", userId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string category = reader.GetString(2);
                        int price = reader.GetInt32(3);
                        int current = reader.GetInt32(4);
                        string image = reader.GetString(5);

                        ret.Add(new Item(id, name, category, price, current, image));
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
