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
                Console.WriteLine("User agent: {0}", request.UserAgent);*/
                Console.WriteLine();

                byte[] data;

                switch (request.HttpMethod)
                {
                    case "GET":
                        data = handleGET(request);
                        break;
                    case "POST":
                        data = handlePOST(request);
                        break;
                    default:
                        //response.StatusCode = 400; TODO
                        data = null;
                        break;
                }

                if (data == null)
                    data = Encoding.UTF8.GetBytes("{}");


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
                case "search":
                    if (request.QueryString["search"] == null)
                        return null;

                    int c = DataBase.getItems(request.QueryString["term"]).Count;
                    return Encoding.UTF8.GetBytes("{status:\"OK\",count=" + c + "}");
                case "ping":
                    return Response.pingResponse();
                default:
                    return Encoding.UTF8.GetBytes("{status:\"OK\"}");
            }
        }

        private byte[] handlePOST(HttpListenerRequest request)
        {
            return Encoding.UTF8.GetBytes("{status:\"OK\"}");
        }

        private string getEndpoint(string url)
        {
            char[] delimiters = { '/' };
            string[] parts = url.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if(parts != null && parts.Length >= 1)
                return parts[0];
            return null;
        }
    }
}
