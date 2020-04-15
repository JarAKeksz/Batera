using Client.Modells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
 
    public class BateraCliensClass
    {
        static readonly HttpClient client = new HttpClient();
        static string testString;
        //public async void Main()
        //{
        //    // Call asynchronous network methods in a try/catch block to handle exceptions.
        //    try
        //    {
        //        HttpResponseMessage response =  client.GetAsync("http://localhost:8000/search?term=a").Result;
        //        response.EnsureSuccessStatusCode();
        //        string responseBody =  response.Content.ReadAsStringAsync().Result;
        //        // Above three lines can be replaced with new helper method below
        //        // string responseBody = await client.GetStringAsync(uri);
        //        Console.WriteLine(responseBody);



        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine("\nException Caught!");
        //        Console.WriteLine("Message :{0} ", e.Message);
        //    }
        //}


        //This class returns Json from server at url
        private string JsonBody(string url)
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string jsonString = response.Content.ReadAsStringAsync().Result;

            return jsonString;
        }

        public List<Item> AllItem()
        {
            List<Item> result = new List<Item>();

            string itemsJson = JsonBody("http://localhost:8000/search");

            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("items").EnumerateArray())
                {

                    int id = element.GetProperty("id").GetInt32();
                    string name = element.GetProperty("name").GetString();


                    result.Add(new Item(id, name));
                }
            }
            return result;
        }

        public List<Item> SearchedItem(string searchedName)
        {
            List<Item> result = new List<Item>();

            string itemsJson = JsonBody("http://localhost:8000/search?term="+ searchedName);

            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("items").EnumerateArray())
                {

                    int id = element.GetProperty("id").GetInt32();
                    string name = element.GetProperty("name").GetString();


                    result.Add(new Item(id, name));
                }
            }
            return result;

        }



    }
}
