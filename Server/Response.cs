using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Server
{
    class Response
    {
        private static readonly JsonWriterOptions JW_OPTS = new JsonWriterOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        private static byte[] getBytes(JsonDocument json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream); //, new JsonWriterOptions { Indented = true }
                json.WriteTo(writer);
                writer.Flush();
                return stream.ToArray();
            }
        }

        private static byte[] getBytes(string json)
        {
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] pingResponse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteString("message", "blin");
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] searchResponse(Dictionary<string, string> searchTerms = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteStartArray("items");
                    List<Item> items = DataBase.getItems(searchTerms);
                    if (items == null) return null;
                    foreach (Item i in items)
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("id", i.id);
                        writer.WriteString("name", i.name);
                        writer.WriteNumber("price", i.price);
                        writer.WriteNumber("current", i.current);
                        writer.WriteString("category", i.category);
                        writer.WriteString("image", i.image);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] categoriesResponse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteStartArray("categories");
                    foreach (Category c in DataBase.getCategories())
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("id", c.id);
                        writer.WriteString("name", c.name);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
        
        public static byte[] favoritesResponse(string token)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteStartArray("items");
                    foreach (Item i in DataBase.getFavorites(token))
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("id", i.id);
                        writer.WriteString("name", i.name);
                        writer.WriteNumber("price", i.price);
                        writer.WriteNumber("current", i.current);
                        writer.WriteString("category", i.category);
                        writer.WriteString("image", i.image);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] notificationsResponse(string token)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteStartArray("notifications");
                    foreach (Notification n in DataBase.getNotifications(token))
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("item_id", n.itemId);
                        writer.WriteString("item_name", n.itemName);
                        writer.WriteString("time", n.timeStamp.ToString());
                        writer.WriteNumber("type", n.textType);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] itemResponse(int id)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();

                    DetailedItem i = DataBase.getItemDetails(id);

                    if (i == null) return null;
                    
                    writer.WriteNumber("id", i.id);
                    writer.WriteString("name", i.name);
                    writer.WriteNumber("start_price", i.bidStart);
                    writer.WriteNumber("min_bid", i.current + i.bidIncrement);
                    writer.WriteNumber("current_price", i.current);
                    writer.WriteBoolean("quick_buy", i.buyWithoutBid);
                    if(i.buyWithoutBid)
                        writer.WriteNumber("buy_price", i.price);
                    writer.WriteBoolean("new", i.isItNew);
                    writer.WriteString("category", i.category);
                    writer.WriteString("image", i.image);
                    writer.WriteString("description", i.description);
                    writer.WriteString("end_date", i.endDate);
                    writer.WriteString("seller", i.seller);
                    writer.WriteString("sold_to", i.soldTo);

                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] signUpResponse(string email, string password, string userName, string name)
        {
            byte b = DataBase.signUp(userName, name, email, password);
            if (b == 0)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                    {
                        writer.WriteStartObject();
                        User u = DataBase.logIn(email, password);
                        if (u == null)
                        {
                            throw new Exception("User registered but cant find none the less, oof");
                        }
                        else
                        {
                            writer.WriteString("token", u.logInToken);
                        }
                        writer.WriteEndObject();
                    }

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        internal static byte[] buyResponse(string token, int itemId)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();

                    byte b = DataBase.buyItem(token, itemId);

                    switch (b)
                    {
                        case 0:
                            writer.WriteBoolean("success", true);
                            break;
                        case 1:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "No quick buy for this item");
                            break;
                        case 2:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Item sale period has ended");
                            break;
                        case 3:
                            throw new Exception("Database error");
                            break;
                        case 4:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "No item with that ID");
                            break;
                        default:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Unknown error");
                            break;
                    }

                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] autobidSubscribeResponse(string token, int itemId, int maxPrice)
        {
            bool b = DataBase.setAutoBid(token, itemId, maxPrice);
            
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    if (b)
                    {
                        writer.WriteBoolean("success", true);
                    }
                    else
                    {
                        writer.WriteBoolean("success", true);
                    }
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] autobidRemoveResponse(string token, int itemId)
        {
            bool b = DataBase.removeAutoBid(token, itemId);

            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    if (b)
                    {
                        writer.WriteBoolean("success", true);
                    }
                    else
                    {
                        writer.WriteBoolean("success", true);
                    }
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] bidResponse(string token, int itemId, int bid)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();

                    byte b = DataBase.addBid(token, itemId, bid);

                    switch (b)
                    {
                        case 4:
                            throw new Exception("Database error");
                            break;
                        case 3:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Item not found!");
                            break;
                        case 2:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Bidding has ended!");
                            break;
                        case 1:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Bid too low!");
                            break;
                        case 0:
                            writer.WriteBoolean("success", true);
                            writer.WriteNumber("price", bid);
                            break;
                        default:
                            writer.WriteBoolean("success", false);
                            writer.WriteString("problem", "Unknown error");
                            break;
                    }

                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] loginResponse(string email, string password)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    User u = DataBase.logIn(email, password);
                    if (u == null) {
                        writer.WriteBoolean("success", false);
                        writer.WriteString("problem", "Email and/or password not correct!");
                    }
                    else
                    {
                        writer.WriteBoolean("success", true);
                        writer.WriteString("token", u.logInToken);
                    }
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
        
        public static byte[] toggleFavoriteResponse(string token, int itemId)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();

                    byte b = DataBase.toggleFavorite(token, itemId);

                    if(b >= 2)
                    {
                        throw new Exception("wtf");
                    }

                    writer.WriteBoolean("is_favorite", b == 1);
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }

        public static byte[] uploadResponse(string token, string name, string description, string imageBase64, int categoryId, int startPrice, int buyPrice)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();

                    int id = DataBase.tokenToId(token);
                    if (id < 0) return null;

                    byte b = DataBase.addItem(name, categoryId, imageBase64, id, description, true, startPrice, buyPrice != -1, buyPrice);

                    if (b != 0) return null; //TODO: success false meg társai?
                    
                    writer.WriteBoolean("success", true);
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
    }
}
