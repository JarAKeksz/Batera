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
                    writer.WriteNumber("price", i.price);
                    writer.WriteNumber("current", i.current);
                    writer.WriteString("category", i.category);
                    writer.WriteString("image", i.image);
                    writer.WriteString("description", i.description);
                    writer.WriteString("end_date", i.endDate);
                    writer.WriteString("seller", i.seller);

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
    }
}
