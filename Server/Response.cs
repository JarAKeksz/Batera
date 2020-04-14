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
        private byte[] getBytes(JsonDocument json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream); //, new JsonWriterOptions { Indented = true }
                json.WriteTo(writer);
                writer.Flush();
                return stream.ToArray();
            }
        }

        private byte[] getBytes(string json)
        {
            return Encoding.UTF8.GetBytes(json);
        }


        public static byte[] pingResponse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    writer.WriteString("message", "blin");
                    writer.WriteEndObject();
                }

                return stream.ToArray();
            }
        }
    }
}
