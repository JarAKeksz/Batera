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
                string tokenCheckQuery = "SELECT Id, UserName, Name, Email FROM Users WHERE Token = @logInToken";
                using (SqlCommand command = new SqlCommand(tokenCheckQuery, connection))
                {
                    command.Parameters.Add("@logInToken", SqlDbType.Char).Value = token;

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string userName = reader.GetString(1);
                        string name = reader.GetString(2);
                        string email = reader.GetString(3);
                        ret = new User(id, userName, token, name, email);
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
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT, @token CHAR(64) " +
                    "SELECT @id = Id, @token = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', CONCAT(Id, UserName, GETDATE())), 2) " +
                    "FROM Users WHERE Email = @email AND PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2) " +
                    "UPDATE Users SET Token = @token WHERE Id = @id " +
                    "SELECT Id, UserName, Token, Name FROM Users WHERE Token = @token " +
                    "COMMIT TRANSACTION";
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
                        string token = reader.GetString(2);
                        string name = reader.GetString(3);
                        ret = new User(id, userName, token, name, email);
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

        public static bool logOut(string token)
        {
            try
            {
                string query = "UPDATE Users SET Token = NULL WHERE Token = @token";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public static byte signUp(string userName, string name, string email, string password)
        {
            try
            {
                string query = "INSERT INTO Users (UserName, Name, Email, PasswordHash) " +
                        "VALUES(@userName, @name, @email, CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @password), 2))";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@userName", userName));
                    command.Parameters.Add(new SqlParameter("@name", name));
                    command.Parameters.Add(new SqlParameter("@email", email));
                    command.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (SqlException e)
            {
                //Console.WriteLine(e);
                if (e.Message.Contains("UserNameCheck")) //haha vibe check
                {
                    return 1; // == van már ilyen nevű felhasználó
                }
                if (e.Message.Contains("EmailCheck"))
                {
                    return 2; // == van már ilyen email
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 3; // == vmi más baj van
            }
            return 0; // == minden rendben
        }

        public static byte addItem(string name, int categoryId, string image, int sellerId, string description, bool isItNew, int bidStart, 
            bool buyWithoutBid = false, int price = -1)
        {
            try
            {
                string query;
                if (buyWithoutBid && price != -1)
                {
                    query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                        "BEGIN TRANSACTION " +
                        "INSERT INTO Items(Name, Seller, CategoryId, Image, Description, EndDate, IsItNew, BuyWithoutBid, Price, BidStart) " +
                        "VALUES(@Name, @Seller, @CategoryId, @Image, @Description, DATEADD(DAY, 7, GETDATE()), @IsItNew, @BuyWithoutBid, @Price, @BidStart) " +
                        "DECLARE @itemId INT " +
                        "SELECT @itemId = Id FROM Items WHERE Name = @Name " +
                        "INSERT INTO Subscriptions(ItemId, UserId) VALUES(@itemId, @Seller) " +
                        "COMMIT TRANSACTION";
                }
                else if (!buyWithoutBid)
                {
                    query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                        "BEGIN TRANSACTION " +
                        "INSERT INTO Items(Name, Seller, CategoryId, Image, Description, EndDate, IsItNew, BidStart) " +
                        "VALUES(@Name, @Seller, @CategoryId, @Image, @Description, DATEADD(DAY, 7, GETDATE()), @IsItNew, @BidStart) " +
                        "DECLARE @itemId INT " +
                        "SELECT @itemId = Id FROM Items WHERE Name = @Name " +
                        "INSERT INTO Subscriptions(ItemId, UserId) VALUES(@itemId, @Seller) " +
                        "COMMIT TRANSACTION";
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
                    command.Parameters.Add(new SqlParameter("@IsItNew", isItNew));
                    command.Parameters.Add(new SqlParameter("@BidStart", bidStart));
                    if (buyWithoutBid && price != -1)
                    {
                        command.Parameters.Add(new SqlParameter("@BuyWithoutBid", buyWithoutBid));
                        command.Parameters.Add(new SqlParameter("@Price", price));
                    }

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                if (e.Message.Contains("NameCheck"))
                {
                    return 2; // == van már ilyen nevű termék
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 3; // == vmi más baj van
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
                        if (key == "Name" || key == "UserName" || key == "Email")
                        {
                            query += key + " = [@" + key + "]";
                        }
                        else if (key == "Password")
                        {
                            query += "PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @Password), 2)";
                        }
                        else
                        {
                            return null;
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
                                if (pair.Key == "Password")
                                {
                                    update.Parameters.Add("@password", SqlDbType.VarChar).Value = pair.Value;
                                }
                                else
                                {
                                    string tmp = "@" + pair.Key;
                                    update.Parameters.Add(new SqlParameter(tmp, pair.Value));
                                }

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
                                query += key + " LIKE '%' + @" + key + " + '%'";
                                break;

                            case "Id":
                                query += key + " = @" + key;
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
                query += " GROUP BY i.Id, i.Name, c.Name, i.Price, i.BidStart, i.Image, i.Active";
                if (eMinBid && eMaxBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) >= @MinBid AND ISNULL(MAX(b.Value),i.BidStart) <= @MaxBid AND i.Active = 1";
                }
                else if (eMinBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) >= @MinBid AND i.Active = 1";
                }
                else if (eMaxBid)
                {
                    query += " HAVING ISNULL(MAX(b.Value),i.BidStart) <= @MaxBid AND i.Active = 1";
                }
                else
                {
                    query += " HAVING i.Active = 1";
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
                int id;
                string name;
                string category;
                int price;
                int current;
                string image;

                string seller;
                string description;
                DateTime endDate;
                bool isItNew;
                bool buyWithoutBid;
                string soldTo;
                int bidStart;
                int bidIncrement;

                string query = "SELECT i.Name, c.Name, ISNULL(i.Price,-1), ISNULL(MAX(b.Value),i.BidStart), i.Image, u.UserName, i.Description, i.EndDate, " +
                    "i.IsItNew, i.BuyWithoutBid, ISNULL(su.UserName,''), i.BidStart, i.BidIncrement " +
                    "FROM Items AS i JOIN Categories AS c ON i.CategoryId = c.Id LEFT JOIN Bids AS b ON i.Id = b.ItemId JOIN Users AS u ON i.Seller = u.Id " +
                    "LEFT JOIN Sales AS s ON i.Id = s.ItemId LEFT JOIN Users AS su ON s.UserId = su.Id WHERE i.Id = @itemId " +
                    "GROUP BY i.Name, c.Name, i.Price, i.BidStart, i.Image, u.UserName, i.Description, i.EndDate, i.IsItNew, i.BuyWithoutBid, su.UserName, i.BidStart, i.BidIncrement";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        id = itemId;
                        name = reader.GetString(0);
                        category = reader.GetString(1);
                        price = reader.GetInt32(2);
                        current = reader.GetInt32(3);
                        image = reader.GetString(4);

                        seller = reader.GetString(5);
                        description = reader.GetString(6);
                        endDate = reader.GetDateTime(7);
                        isItNew = (bool)reader[8];
                        buyWithoutBid = (bool)reader[9];
                        soldTo = reader.GetString(10);
                        bidStart = reader.GetInt32(11);
                        bidIncrement = (int)reader.GetDecimal(12);
                    }
                    else
                    {
                        return null;
                    }

                    reader.Close();
                }

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

                ret = new DetailedItem(id, name, category, price, current, image, seller, description, endDate, isItNew, buyWithoutBid, soldTo, bidStart, bidIncrement, tmp);
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

        public static List<Item> getFavorites(string token)
        {
            List<Item> ret = new List<Item>();

            try
            {
                string query = "SELECT f.ItemId, i.Name, c.Name, ISNULL(i.Price,-1), ISNULL(MAX(b.Value),i.BidStart), i.Image " +
                    "FROM Favorites AS f JOIN Items AS i ON f.ItemId = i.Id JOIN Categories AS c ON i.CategoryId = c.Id " +
                    "LEFT JOIN Bids AS b ON i.Id = b.ItemId LEFT JOIN Users AS u ON f.UserId = u.Id WHERE u.Token = @token " +
                    "GROUP BY f.ItemId, i.Name, c.Name, i.Price, i.BidStart, i.Image";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));

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

        public static byte toggleFavorite(string token, int itemId)
        {
            bool favorite = false;
            try
            {
                string favoriteCheckQuery = "SELECT f.ItemId FROM Favorites AS f RIGHT JOIN Users AS u ON f.UserId = u.Id " +
                    "WHERE u.Token = @token AND f.ItemId = @itemId";
                using (SqlCommand command = new SqlCommand(favoriteCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        favorite = reader.GetInt32(0) != -1;
                    }
                    reader.Close();
                }
                if(favorite)
                {
                    string query = "BEGIN TRANSACTION " +
                        "DECLARE @id INT " +
                        "SELECT @id = Id FROM Users WHERE Token = @token " +
                        "DELETE Favorites WHERE ItemId = @itemId AND UserId = @id " +
                        "COMMIT TRANSACTION";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@token", token));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    favorite = false;
                }
                else
                {
                    string query = "BEGIN TRANSACTION " +
                        "DECLARE @id INT " +
                        "SELECT @id = Id FROM Users WHERE Token = @token " +
                        "INSERT INTO Favorites (ItemId, UserId) VALUES(@itemId, @id) " +
                        "COMMIT TRANSACTION";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@token", token));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    favorite = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 2;
            }
            return Convert.ToByte(favorite);
        }

        public static List<Item> getBidsByUser(string token)
        {
            List<Item> ret = new List<Item>();

            try
            {
                string query = "SELECT b.ItemId, i.Name, c.Name, ISNULL(i.Price,-1), b.Value, i.Image " +
                    "FROM Bids AS b JOIN Items AS i ON b.ItemId = i.Id JOIN Categories AS c ON i.CategoryId = c.Id " +
                    "LEFT JOIN Users AS u ON f.UserId = u.Id WHERE u.Token = @token " +
                    "GROUP BY b.ItemId, i.Name, c.Name, i.Price, b.Value, i.BidStart, i.Image";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));

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

        public static byte addBid(string token, int itemId, int value)
        {
            try
            {
                string thresholdCheckQuery = "SELECT TOP (1) i.EndDate, ISNULL(b.Value,i.BidStart)+i.BidIncrement FROM Items AS i " +
                    "LEFT JOIN Bids AS b ON b.ItemId = i.Id WHERE i.Id = @itemId AND i.Active = 1 ORDER BY ISNULL(b.Value,i.BidStart) DESC";
                using (SqlCommand command = new SqlCommand(thresholdCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    int tmp = -1;
                    DateTime tmpDate = new DateTime();
                    if (reader.Read())
                    {
                        tmpDate = reader.GetDateTime(0);
                        tmp = (int)reader.GetDecimal(1);
                    }
                    reader.Close();
                    if (tmp == -1)
                    {
                        return 3; // == nem található a termék
                    }
                    if (tmpDate <= DateTime.Now)
                    {
                        return 2; // == a termékre már nem érkezhet licit
                    }
                    if (tmp > value)
                    {
                        return 1; // == a megadott licit nem éri el a küszöböt
                    }
                }
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT " +
                    "SELECT @id = Id FROM Users WHERE Token = @token " +
                    "INSERT INTO Bids(ItemId, UserId, Value) VALUES(@itemId, @id, @value) " +
                    "INSERT INTO Subscriptions (ItemId, UserId) VALUES(@itemId, @id) " +
                    "DECLARE @bidJump INT " +
                    "SELECT @bidJump = BidIncrement FROM Items WHERE Id = @itemId " +
                    "INSERT INTO Bids(ItemId, UserId, Value) " +
                    "SELECT a.UserId, a.ItemId, MAX(b.Value)+@bidJump FROM AutoBids AS a " +
                    "JOIN Bids AS b ON a.ItemId = b.ItemId " +
                    "WHERE a.ItemId = @itemId AND a.Limit > MAX(b.Value)+@bidJump " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '5' FROM Autobid " +
                    "WHERE ItemId = @itemId AND UserId != @id " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '0' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId != @id " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));
                    command.Parameters.Add(new SqlParameter("@value", value));

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 4; // == oops
            }
            return 0;
        }

        public static bool setAutoBid(string token, int itemId, int limit)
        {
            try
            {
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT " +
                    "SELECT @id = Id FROM Users WHERE Token = @token " +
                    "INSERT INTO AutoBids(ItemId, UserId, Value) VALUES(@itemId, @id, @limit) " +
                    "INSERT INTO Subscriptions (ItemId, UserId) VALUES(@itemId, @id) " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));
                    command.Parameters.Add(new SqlParameter("@value", limit));

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false; // == oops
            }
            return true;
        }

        public static bool removeAutoBid(string token, int itemId)
        {
            try
            {
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT " +
                    "SELECT @id = Id FROM Users WHERE Token = @token " +
                    "DELETE AutoBids WHERE ItemId = @itemId AND UserId = @id " +
                    "DELETE Subscriptions WHERE ItemId = @itemId AND UserId = @id " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false; // == oops
            }
            return true;
        }

        public static List<Notification> getNotifications(string token)
        {
            List<Notification> ret = new List<Notification>();

            try
            {
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT " +
                    "SELECT @id = Id FROM Users WHERE Token = @token " +
                    "SELECT n.Id, n.ItemId, i.Name, n.TimeStamp, n.TextType INTO #tmp FROM Notifications AS n " +
                    "JOIN Items AS i ON n.ItemId = i.Id WHERE n.UserId = @id " +
                    "DELETE Notifications WHERE UserId = @id " +
                    "SELECT * FROM #tmp " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int itemId = reader.GetInt32(1);
                        string itemName = reader.GetString(2);
                        DateTime timeStamp = reader.GetDateTime(3);
                        byte textType = reader.GetByte(4);

                        ret.Add(new Notification(id, itemId, itemName, timeStamp, textType));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return ret;
        }

        public static List<int> getSubscriptions(string token)
        {
            List<int> ret = new List<int>();

            try
            {
                string query = "SELECT s.ItemId FROM Subscriptions AS s LEFT JOIN Users AS u ON s.UserId = u.Id " +
                    "WHERE u.Token = @token";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ret.Add(id);
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

        public static byte toggleSubscription(string token, int itemId)
        {
            bool subscribed = false;
            try
            {
                string subscriptionCheckQuery = "SELECT s.ItemId FROM Subscriptions AS s RIGHT JOIN Users AS u ON s.UserId = u.Id " +
                    "WHERE u.Token = @token AND s.ItemId = @itemId";
                using (SqlCommand command = new SqlCommand(subscriptionCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        subscribed = reader.GetInt32(0) != -1;
                    }
                    reader.Close();
                }
                if (subscribed)
                {
                    string query = "BEGIN TRANSACTION " +
                        "DECLARE @id INT " +
                        "SELECT @id = Id FROM Users WHERE Token = @token " +
                        "DELETE Subscriptions WHERE ItemId = @itemId AND UserId = @id " +
                        "DELETE Notifications WHERE ItemId = @itemId AND UserId = @id " +
                        "COMMIT TRANSACTION";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@token", token));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    subscribed = false;
                }
                else
                {
                    string query = "BEGIN TRANSACTION " +
                        "DECLARE @id INT " +
                        "SELECT @id = Id FROM Users WHERE Token = @token " +
                        "INSERT INTO Subscriptions (ItemId, UserId) VALUES(@itemId, @id) " +
                        "COMMIT TRANSACTION";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@token", token));
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));

                        Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                    }
                    subscribed = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 2;
            }
            return Convert.ToByte(subscribed);
        }

        public static bool getEndedSaleNotifications()
        {
            try
            {
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @itemId INT, @sellerId INT, @winnerId INT " +
                    "DECLARE cur CURSOR LOCAL FOR " +
                    "SELECT Id, Seller FROM Items " +
                    "WHERE EndDate <= GETDATE() AND Active = 1 " +
                    "OPEN cur " +
                    "FETCH NEXT FROM cur into @itemId, @sellerId " +
                    "WHILE @@FETCH_STATUS = 0 " +
                    "BEGIN " +
                    "UPDATE Items SET Active = 0 WHERE Id = @itemId " +
                    "SELECT @winnerId = UserId FROM Bids " +
                    "WHERE Value IN (SELECT MAX(Value) FROM Bids GROUP BY ItemId) " +
                    "AND ItemId = @itemId " +
                    "IF @winnerId IS NOT NULL " +
                    "BEGIN " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '1' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId != @winnerId AND UserId != @sellerId " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '2' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId = @winnerId " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '4' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId = @sellerId " +
                    "END " +
                    "ELSE " +
                    "BEGIN " +
                    "INSERT INTO Notifications (UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '3' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId = @sellerId " +
                    "END " +
                    "DELETE Subscriptions WHERE ItemId = @itemId " +
                    "FETCH NEXT FROM cur into @itemId, @sellerId " +
                    "END " +
                    "CLOSE cur " +
                    "DEALLOCATE cur " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false; // == oops
            }
            return true;
        }

        public static byte buyItem(string token, int itemId)
        {
            try
            {
                string thresholdCheckQuery = "SELECT EndDate, BuyWithoutBid FROM Items WHERE Id = @itemId AND Active = 1";
                using (SqlCommand command = new SqlCommand(thresholdCheckQuery, connection))
                {
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    DateTime tmpDate = new DateTime(1970,1,1,0,0,0);
                    bool buyWithoutBid = false;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        tmpDate = reader.GetDateTime(0);
                        buyWithoutBid = (bool)reader[1];
                    }
                    reader.Close();
                    if (tmpDate <= DateTime.Now)
                    {
                        return 2; // == a terméket már nem lehet megvenni
                    }
                    if (!buyWithoutBid)
                    {
                        return 1; // == a terméket nem lehetséges megvenni
                    }
                }
                string query = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED " +
                    "BEGIN TRANSACTION " +
                    "DECLARE @id INT " +
                    "SELECT @id = Id FROM Users WHERE Token = @token " +
                    "INSERT INTO Sales(ItemId, UserId) VALUES(@itemId, @id) " +
                    "DECLARE @sellerId INT " +
                    "SELECT @sellerId = Seller FROM Items WHERE Id = @itemId " +
                    "UPDATE Items SET Active = 0 WHERE Id = @itemId " +
                    "INSERT INTO Notifications(UserId, ItemId, TimeStamp, TextType) " +
                    "SELECT UserId, ItemId, GETDATE(), '1' FROM Subscriptions " +
                    "WHERE ItemId = @itemId AND UserId != @id AND UserId != @sellerId " +
                    "INSERT INTO Notifications(UserId, ItemId, TimeStamp, TextType) " +
                    "VALUES(@id, ItemId, GETDATE(), '2') " +
                    "INSERT INTO Notifications(UserId, ItemId, TimeStamp, TextType) " +
                    "VALUES(@sellerId, ItemId, GETDATE(), '4') " +
                    "DELETE Subscriptions WHERE ItemId = @itemId " +
                    "COMMIT TRANSACTION";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));
                    command.Parameters.Add(new SqlParameter("@itemId", itemId));

                    Console.WriteLine("Erintett sorok: " + command.ExecuteNonQuery());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 3; // == oops
            }
            return 0;
        }

        public static int tokenToId(string token)
        {
            int id = -1;
            try
            {
                string query = "SELECT Id FROM Users WHERE Token = @token";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@token", token));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -2;
            }
            return id;
        }
    }
}

