using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meteo
{

    class Program
    {

        public static string sAppName = "App  : Weather Info 1.0.0";
        public static string sUserName = "User : <not logged>";
        public static string sUserRole = "Role : <undefined>";
        public static string sAppMenu = "Menu : Main";
        public static string sAppHeader = "";
        public static string api_key = "cfdb595e4553142ce380ac44c3bad4d6" ;
       public static async Task Main(string[] args)
        {

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    //Console.WriteLine($"Argument={arg}");
                    if (arg.ToLower() == "apikey") {
                        
                    }
                }
            }

            sAppHeader = $"{sAppName}";

            MenuMain mnMain = new MenuMain(api_key, sAppHeader);

            mnMain.Run();

        }

    }
}