using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
   public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
       


    }
    public class BateraCliensClass
    {
        static readonly HttpClient client = new HttpClient();
        static string testString;
        public async void Main()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response =  client.GetAsync("http://localhost:8000/").Result;
                response.EnsureSuccessStatusCode();
                string responseBody =  response.Content.ReadAsStringAsync().Result;
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                Console.WriteLine("*******************");
                Console.WriteLine(responseBody);


                
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
       

    }
}
