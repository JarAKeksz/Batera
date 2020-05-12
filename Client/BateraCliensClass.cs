using Client.Modells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Client
{
 
    public class BateraCliensClass
    {
        static readonly HttpClient client = new HttpClient();
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
            string jsonString = null;
            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                Console.WriteLine(response);
                response.EnsureSuccessStatusCode();
                jsonString = response.Content.ReadAsStringAsync().Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e); ;
            }
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
            if( itemsJson != null) { 
                using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
                {
                    foreach (JsonElement element in document.RootElement.GetProperty("items").EnumerateArray())
                    {

                        int id = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();
                        int price = element.GetProperty("price").GetInt32();
                        string category = element.GetProperty("category").GetString();
                        string image = element.GetProperty("image").GetString();

                        result.Add(new Item(id, name, price, category, image));
                    }
                }
            }

            return result;
        }


        public List<Item> SearchedItem(string searchedName, int searchedCategory, string selectedBuyingFormat, string selectedCondition, int minPrice, int maxPrice, int selectedCategory)
        {
            List<Item> result = new List<Item>();

            var builder = new UriBuilder("http://localhost:8000/search?");
            builder.Port = 8000;
            var query = HttpUtility.ParseQueryString(builder.Query);
            if (searchedName!= "")
            {
                query["Name"] = searchedName;
            }
            if(selectedBuyingFormat != "All") {
                if (selectedBuyingFormat == "Bid")
                {
                    query["BuyWithoutBid"] = "false";
                }
                else
                {
                    query["BuyWithoutBid"] = "true";
                }
            }
            if (selectedCondition != "All")
            {
                if (selectedBuyingFormat == "New")
                {
                    query["IsItNew"] = "true";
                }
                else
                {
                    query["IsItNew"] = "false";
                }
            }
            if (minPrice != -1)
            {
                query["MinPrice"] = minPrice.ToString();
            }
            if (maxPrice != -1)
            {
                query["MaxPrice"] = maxPrice.ToString();
            }
            if(selectedCategory != -1)
            {
                query["CategoryId"] = selectedCategory.ToString();
            }

                builder.Query = query.ToString();
            string url = builder.ToString();



            string itemsJson = JsonBody(url);
            Console.WriteLine(url);


            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            if(itemsJson != null) { 
                using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
                {
                    JsonElement element = document.RootElement;
                    

                        int id = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();
                        int price = element.GetProperty("price").GetInt32();
                        string category = element.GetProperty("category").GetString();
                        string image = element.GetProperty("image").GetString();

                        result.Add(new Item(id, name, price, category, image));
                    
                }
            }


            return result;

        }

        public List<String> GetCategories()
        {
            List<string> result = new List<string>();

            string itemsJson = JsonBody("http://localhost:8000/categories");
            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            if (itemsJson != null)
            {
                using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
                {
                    foreach (JsonElement element in document.RootElement.GetProperty("categories").EnumerateArray())
                    {

                        int id = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();

                        result.Add(name);
                    }
                }
            }


            return result;
        }

        public DetailedItem GetDetailedItem(int id)
        {
            DetailedItem result = null;
            string itemsJson = JsonBody($"http://localhost:8000/item?id={id.ToString()}");
            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };
            if (itemsJson != null)
            {
                using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
                {
                    JsonElement element = document.RootElement;
                    

                        int id2 = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();
                        int price = element.GetProperty("price").GetInt32();
                        string category = element.GetProperty("category").GetString();
                        string image = element.GetProperty("image").GetString();
                        string description = element.GetProperty("description").GetString();
                        string end_date = element.GetProperty("end_date").GetString();
                        string seller = element.GetProperty("seller").GetString();
                        result = (new DetailedItem(id2, name, price, category, image,description,end_date,seller));
                    
                }
            }

            return result;
        }


        public List<Item> FavoriteItem(string id)
        {
            List<Item> result = new List<Item>();

            string itemsJson = JsonBody($"http://localhost:8000/favorites?id={id}");

            //json parse 
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };
            if (itemsJson != null)
            {
                using (JsonDocument document = JsonDocument.Parse(itemsJson, options))
                {
                    foreach (JsonElement element in document.RootElement.GetProperty("items").EnumerateArray())
                    {

                        int id2 = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();
                        int price = element.GetProperty("price").GetInt32();
                        string category = element.GetProperty("category").GetString();
                        string image = element.GetProperty("image").GetString();

                        result.Add(new Item(id2, name, price, category, image));
                    }
                }
            }

            return result;
        }
    }
}












