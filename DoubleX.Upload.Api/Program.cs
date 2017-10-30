using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Owin.Hosting;

namespace DoubleX.Upload.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Initializing webapi application...");
            string baseUri = "http://localhost:8080";
            if (ConfigurationManager.AppSettings.Keys != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Url"].ToString()))
            {
                baseUri = ConfigurationManager.AppSettings["Url"].ToString();
            }
            Console.WriteLine("Starting web Server...");
            WebApp.Start<Startup>(baseUri);
            Console.WriteLine("Server running at {0} - press Enter to quit. ", baseUri);
            Console.ReadLine();
        }
    }
}
