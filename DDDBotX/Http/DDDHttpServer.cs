using DDDBotX.Http.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Http
{
    public class DDDHttpServer
    {
        public const int HTTP_SERVER_PORT = 43233;

        public static Task RunAsync()
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    IPAddress addr = IPAddress.Any;
                    options.Listen(addr, HTTP_SERVER_PORT);

                })
                .UseStartup<DDDHttpServer>()
                .Configure(Configure)
                .Build();

            return host.RunAsync();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.Run(OnHTTPRequest);
        }

        public static async Task OnHTTPRequest(HttpContext e)
        {
            //Do CORS stuff
            e.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization");
            e.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            e.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, DELETE, PUT, PATCH");
            if (e.Request.Method.ToUpper() == "OPTIONS")
            {
                await WriteStringToBody(e, "Dropping OPTIONS request. Hello CORS!", "text/plain", 200);
                return;
            }

            //Run
            try
            {
                if (e.Request.Path == "/status" && e.Request.Method.ToUpper() == "GET")
                    await HttpCurrentStatus.OnHTTPRequest(e);
                else if (e.Request.Path == "/leaderboard" && e.Request.Method.ToUpper() == "GET")
                    await HttpLeaderboard.OnHTTPRequest(e);
                else if (e.Request.Path == "/search_players" && e.Request.Method.ToUpper() == "POST")
                    await HttpPlayerSearch.OnHTTPRequest(e);
                else if (e.Request.Path == "/report_player" && e.Request.Method.ToUpper() == "POST")
                    await HttpPlayerReport.OnHTTPRequest(e);
                else
                    await WriteStringToBody(e, "Not Found", code: 404);
            }
            catch (Exception ex)
            {
                //TODO: Log this
                await WriteStringToBody(e, "Internal Server Error - Try again later", "text/plain", 500);
                Console.WriteLine($"SERVER ERROR {ex.Message} @ {ex.StackTrace}");
            }
        }

        public static int? TryGetNullableIntFromQuery(HttpContext e, string name)
        {
            if (!e.Request.Query.ContainsKey(name))
                return null;
            return int.Parse(e.Request.Query[name]);
        }

        public static int TryGetIntOrDefaultFromQuery(HttpContext e, string name, int defaultValue)
        {
            if (!e.Request.Query.ContainsKey(name))
                return defaultValue;
            return int.Parse(e.Request.Query[name]);
        }

        public static async Task WriteJSONToBody<T>(HttpContext e, T data, int code = 200)
        {
            await WriteStringToBody(e, JsonConvert.SerializeObject(data, Formatting.Indented), "application/json", code);
        }

        public static async Task WriteStringToBody(HttpContext e, string data, string type = "text/plain", int code = 200)
        {
            var response = e.Response;
            response.StatusCode = code;
            response.ContentType = type;
            var bytes = Encoding.UTF8.GetBytes(data);
            response.ContentLength = bytes.Length;
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        public static async Task<T> DecodePOSTBody<T>(HttpContext e)
        {
            //Read stream
            string buffer;
            using (StreamReader sr = new StreamReader(e.Request.Body))
                buffer = await sr.ReadToEndAsync();

            //Assume this is JSON
            return JsonConvert.DeserializeObject<T>(buffer);
        }
    }
}
