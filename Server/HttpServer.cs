using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    class HttpServer
    {
        private HttpListener listener;
        private string url;

        public HttpServer(int port)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("OS not supported.");
                return;
            }

            url = "http://localhost:" + port + "/";
            
            listener = new HttpListener();
            listener.Prefixes.Add(url);
        }

        public void run()
        {
            if (!DataBase.connect())
            {
                Console.WriteLine("Error, database not connected!");
                return;
            }

            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + request.Url.ToString());

                // Display the URL used by the client.
                /*Console.WriteLine("URL: {0}", request.Url.OriginalString);
                Console.WriteLine("Raw URL: {0}", request.RawUrl);
                Console.WriteLine("Query: {0}", request.QueryString);

                // Display the referring URI.
                Console.WriteLine("Referred by: {0}", request.UrlReferrer);

                //Display the HTTP method.
                Console.WriteLine("HTTP Method: {0}", request.HttpMethod);
                //Display the host information specified by the client;
                Console.WriteLine("Host name: {0}", request.UserHostName);
                Console.WriteLine("Host address: {0}", request.UserHostAddress);
                Console.WriteLine("User agent: {0}", request.UserAgent);
                Console.WriteLine();*/

                byte[] data;

                try
                {
                    switch (request.HttpMethod)
                    {
                        case "GET":
                            data = handleGET(request);
                            break;
                        case "POST":
                            data = handlePOST(request);
                            break;
                        default:
                            data = null;
                            break;
                    }

                    if (data == null)
                    {
                        data = Encoding.UTF8.GetBytes("{}");
                        response.StatusCode = 400;
                    }
                }catch(Exception e)
                {
                    data = Encoding.UTF8.GetBytes("{}");
                    response.StatusCode = 500;
                }


                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = data.LongLength;
                
                response.OutputStream.Write(data, 0, data.Length);
                response.Close();
            }

            listener.Close();
        }

        private byte[] handleGET(HttpListenerRequest request)
        {
            switch (getEndpoint(request.RawUrl))
            {
                case "ping":
                    return Response.pingResponse();

                case "search":
                    if (request.QueryString.HasKeys())
                    {
                        Dictionary<string, string> parameters = request.QueryString.Keys.Cast<string>().ToDictionary(k => k, v => request.QueryString[v]);
                        return Response.searchResponse(parameters);
                    }
                    return Response.searchResponse();

                case "categories":
                    return Response.categoriesResponse();

                case "favorites":
                    string token;
                    if (request.QueryString["token"] != null)
                    {
                        token = Convert.ToString(request.QueryString["token"]); //TODO: itten try meg ilyenek
                        return Response.favoritesResponse(token);
                    }
                    else
                    {
                        return null;
                    }

                case "item":
                    int id;
                    if (request.QueryString["id"] != null)
                    {
                        id = Convert.ToInt32(request.QueryString["id"]); //TODO: itten try meg ilyenek
                        return Response.itemResponse(id);
                    }
                    else
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }

        private byte[] handlePOST(HttpListenerRequest request)
        {
            switch (getEndpoint(request.RawUrl))
            {
                case "login":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            return Response.loginResponse(document.RootElement.GetProperty("email").GetString(),
                                                          document.RootElement.GetProperty("password").GetString());
                        }
                    }

                default:
                    return null;
            }
        }

        private string getEndpoint(string url)
        {
            char[] delimiters = { '/', '?', '&' };
            string[] parts = url.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if(parts != null && parts.Length >= 1)
                return parts[0];
            return null;
        }
    }
}
