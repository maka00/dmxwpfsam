using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace dxmapp
{
    struct WebsiteData
    {
        public string Url;
        public string Data;
    }

    class Grabber
    {
        private static List<string> PrepData()
        {
            List<string> result = new List<string>
            {
                "https://www.yahoo.com"
                ,"https://www.google.com"
                ,"https://www.microsoft.com"
                ,"https://www.github.com"
                ,"https://www.heise.de"
            };
            return result;
        }

        private WebsiteData DownloadWebsite(string websiteUrl)
        {
            WebsiteData data;
            WebClient client = new WebClient();
            data.Url = websiteUrl;
            data.Data = client.DownloadString(websiteUrl);
            return data;
        }
        public async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteData results = await Task.Run(() => DownloadWebsite(site));
                Console.WriteLine($"{results.Url} with {results.Data.Length} characters.");
            }
        }
        public async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteData>> tasks = new List<Task<WebsiteData>>();

            foreach (string site in websites)
            {
                tasks.Add(Task.Run(() => DownloadWebsite(site)));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                Console.WriteLine($"{item.Url} with {item.Data.Length} characters.");
            }
        }

        public async Task DoIt()
        {
            await RunDownloadParallelAsync();
        }

    }
    class Program
    {
         static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            logger.Info("Hello");
            Console.WriteLine("Hello World!");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Grabber grabber = new Grabber();
            grabber.DoIt().Wait();
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            Console.WriteLine($"it took {elapsed} ms.");
            LogManager.Shutdown();
        }
    }
}
