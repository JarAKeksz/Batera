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

        public static byte[] searchResponse(string term)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, JW_OPTS))
                {
                    writer.WriteStartObject();
                    writer.WriteStartArray("items");
                    foreach (Item i in DataBase.getItems(term))
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("id", i.id);
                        writer.WriteString("name", i.name);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
    }
}
