using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

                Console.WriteLine(request.Url.ToString());
                Console.WriteLine(request.HttpMethod);
                Console.WriteLine(request.UserHostName);
                Console.WriteLine(request.UserAgent);
                Console.WriteLine();

                byte[] data;

                if (request.HttpMethod == "GET")
                {
                    if(request.QueryString["search"] != null)
                    {
                        int c = DataBase.getItems(request.QueryString["search"]).Count;
                        data = Encoding.UTF8.GetBytes("{status:\"OK\",count=" + c + "}");
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes("{status:\"OK\"}");
                    }
                }
                else
                {
                    data = Encoding.UTF8.GetBytes("{status:\"OK\"}");
                }

                
                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = data.LongLength;
                
                response.OutputStream.Write(data, 0, data.Length);
                response.Close();
            }

            listener.Close();
        }
    }
}
