using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
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

            Thread minuteTasksThread = new Thread(delegate ()
            {
                if (!DataBase.getEndedSaleNotifications())
                {
                    Console.WriteLine("Couldn't generate sale end notifications!");
                }
                Thread.Sleep(60000);
            });
            minuteTasksThread.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + request.Url.ToString());

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
                    Console.WriteLine(e);
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
                        token = request.QueryString["token"];
                        return Response.favoritesResponse(token);
                    }
                    else
                    {
                        return null;
                    }

                case "notifications":
                    if (request.QueryString["token"] != null)
                    {
                        token = request.QueryString["token"];
                        return Response.notificationsResponse(token);
                    }
                    else
                    {
                        return null;
                    }

                case "item":
                    int id;
                    if (request.QueryString["id"] != null)
                    {
                        id = Convert.ToInt32(request.QueryString["id"]);
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

                case "sign_up":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            return Response.signUpResponse(document.RootElement.GetProperty("email").GetString(),
                                                          document.RootElement.GetProperty("password").GetString(),
                                                          document.RootElement.GetProperty("username").GetString(),
                                                          document.RootElement.GetProperty("name").GetString());
                        }
                    }

                case "bid":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            return Response.bidResponse(document.RootElement.GetProperty("token").GetString(),
                                                          document.RootElement.GetProperty("item_id").GetInt32(),
                                                          document.RootElement.GetProperty("bid").GetInt32());
                        }
                    }

                case "toggle_favorite":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            return Response.toggleFavoriteResponse(document.RootElement.GetProperty("token").GetString(),
                                                          document.RootElement.GetProperty("item_id").GetInt32());
                        }
                    }

                case "autobid":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            if (document.RootElement.GetProperty("subscribe").GetBoolean()) {
                                return Response.autobidSubscribeResponse(document.RootElement.GetProperty("token").GetString(),
                                                                   document.RootElement.GetProperty("item_id").GetInt32(),
                                                                   document.RootElement.GetProperty("max_bid").GetInt32());
                            }
                            else
                            {
                                return Response.autobidRemoveResponse(document.RootElement.GetProperty("token").GetString(),
                                                                   document.RootElement.GetProperty("item_id").GetInt32());
                            }
                        }
                    }

                case "upload":
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        string s = reader.ReadToEnd();
                        using (JsonDocument document = JsonDocument.Parse(s))
                        {
                            return Response.uploadResponse(document.RootElement.GetProperty("token").GetString(), 
                                                          document.RootElement.GetProperty("name").GetString(),
                                                          document.RootElement.GetProperty("description").GetString(),
                                                          document.RootElement.GetProperty("image").GetString(),
                                                          document.RootElement.GetProperty("category_id").GetInt32(),
                                                          document.RootElement.GetProperty("start_price").GetInt32(),
                                                          document.RootElement.GetProperty("buy_price").GetInt32());
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
