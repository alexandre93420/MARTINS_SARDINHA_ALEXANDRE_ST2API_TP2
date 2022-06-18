using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meteo
{
    public class MenuMain
    {
        public enum Option
        {
            Quit = 0,
            WeatherNow = 1,
            SunRiseSunSetNow = 2,
            TemperatureNow = 3,
            WindInCities = 4,
            PressureHumidityInCities = 5,
        }
        private bool bInMenuMain = false;
        private Option eOptionSelected = Option.Quit;
        public string Name { get; set; }
        public string Header { get; set; }
        public string ApiKey { get; set; }
        public string ApiResponse { get; set; }
        public bool waiting { get; set; }

        public MenuMain(string key)
        {
            this.Name = "Main";
            this.Header = $"Menu : {this.Name}";
            this.ApiKey = key;
            this.ApiResponse = "";
        }

        public MenuMain(string key, string hd)
        {

            this.Name = "Main";
            this.Header = $"{hd}\nMenu : {this.Name}";
            this.ApiKey = key;
            this.ApiResponse = "";
        }

        public void Show()
        {

            Console.Clear();
            Console.WriteLine("------------------------------");
            Console.WriteLine(this.Header);
            Console.WriteLine("------------------------------");
            Console.WriteLine("");
            Console.WriteLine("1 : Weather now in city");
            Console.WriteLine("2 : Sun rise and sun set today in city");
            Console.WriteLine("3 : Tempeature now in city");
            Console.WriteLine("4 : Most windy among cities");
            Console.WriteLine("5 : Pressure and humidity in cities");
            Console.WriteLine("");
            Console.WriteLine("0 : Quit the application");
            Console.WriteLine("");
            Console.WriteLine("------------------------------");
            Console.Write("Enter your choice : ");

        }

        public Option GetChoice()
        {

            // add try/catch
            var choice = Convert.ToInt32(Console.ReadLine());

            while (choice < 0 || choice > 5)
            {
                Console.WriteLine("ERROR: Please enter a correct number!");
                Console.Write("Enter your choice : ");
                choice = Convert.ToInt32(Console.ReadLine());
            }

            return (Option)choice;
        }

        public void Run()
        {

            this.bInMenuMain = true;

            while (bInMenuMain)
            {
                this.Show();
                eOptionSelected = this.GetChoice();

                switch (eOptionSelected)
                {
                    case Option.WeatherNow:

                        WeatherNow();
                        
                        break;

                    case Option.SunRiseSunSetNow:

                        SunRiseSunSetNow();
                        
                        break;

                    case Option.TemperatureNow:
                        
                        TemperatureNow();                        
                        break;

                    case Option.WindInCities:
                        
                        WindInCities();                        
                        break;

                    case Option.PressureHumidityInCities:

                        Console.WriteLine("[INF]: Option PressureHumidityInCities not implemented!");
                        Console.WriteLine("<press [enter] to continue>");
                        Console.ReadLine();                        
                        break;

                    case Option.Quit:
                        Console.WriteLine("\nGoodbye ...\n");
                        bInMenuMain = false;
                        break;
                }
            }
        }

        public async Task CallApi(string uri)
        {

            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri)
                };
                using (var response = await client.SendAsync(request))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        this.ApiResponse = await response.Content.ReadAsStringAsync();
                        this.waiting = false ;
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine("[ERR]: HttpRequestException!");
                        Console.WriteLine("<press [enter] to exit>");
                        Console.ReadLine();
                        Environment.Exit(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERR]: SendAsync!");
                Console.WriteLine("<press [enter] to exit>");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }
        public void WeatherNow() {

            int counter = 0 ;

            //Console.Write("Enter country code: ");
            ///string country = Console.ReadLine();
            Console.Write("Enter city name: ");
            string sCity = Console.ReadLine();

            this.waiting = true ;

            CallApi($"http://api.openweathermap.org/geo/1.0/direct?q={sCity}&limit=10&appid={this.ApiKey}");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            List<GeoLocationRoot> cities = JsonSerializer.Deserialize<List<GeoLocationRoot>>(this.ApiResponse);
            Console.WriteLine("Choose the right country:");

            foreach (var city in cities) {
                Console.WriteLine($"{counter++} : Country = {city.Country}, State = {city.State}, Longitude = {city.Lon}, Latitude = {city.Lat}");
            }
            
            Console.Write("Enter your choice: ");
            var choice = Convert.ToInt32(Console.ReadLine());

            while (choice < 0 || choice > (counter-1))
            {
                Console.WriteLine("ERROR: Please enter a correct number!");
                Console.Write("Enter your choice : ");
                choice = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine($"Name = {cities[choice].Name} : Country = {cities[choice].Country}, State = {cities[choice].State}, Longitude = {cities[choice].Lon}, Latitude = {cities[choice].Lat}");

            this.waiting = true ;
            string exclude = "minutely,hourly,daily,alerts" ;

            CallApi($"https://api.openweathermap.org/data/2.5/onecall?lat={cities[choice].Lat}&lon={cities[choice].Lon}&exclude={exclude}&appid={this.ApiKey}");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            OneCallRoot WeatherInfo = JsonSerializer.Deserialize<OneCallRoot>(this.ApiResponse);

            Console.WriteLine($"Weather = {WeatherInfo.Current.Weather[0].Main}, Description = {WeatherInfo.Current.Weather[0].Description}");

            Console.WriteLine("<press [enter] to exit>");
            Console.ReadLine();

        }
    
        public void SunRiseSunSetNow() {

            int counter = 0 ;
            DateTime SunSet, SunRise ;

            //Console.Write("Enter country code: ");
            ///string country = Console.ReadLine();
            Console.Write("Enter city name: ");
            string sCity = Console.ReadLine();

            this.waiting = true ;

            CallApi($"http://api.openweathermap.org/geo/1.0/direct?q={sCity}&limit=10&appid={this.ApiKey}");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            List<GeoLocationRoot> cities = JsonSerializer.Deserialize<List<GeoLocationRoot>>(this.ApiResponse);
            Console.WriteLine("Choose the right country:");

            foreach (var city in cities) {
                Console.WriteLine($"{counter++} : Country = {city.Country}, State = {city.State}, Longitude = {city.Lon}, Latitude = {city.Lat}");
            }
            
            Console.Write("Enter your choice: ");
            var choice = Convert.ToInt32(Console.ReadLine());

            while (choice < 0 || choice > (counter-1))
            {
                Console.WriteLine("ERROR: Please enter a correct number!");
                Console.Write("Enter your choice : ");
                choice = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine($"Name = {cities[choice].Name} : Country = {cities[choice].Country}, State = {cities[choice].State}, Longitude = {cities[choice].Lon}, Latitude = {cities[choice].Lat}");

            this.waiting = true ;
            string exclude = "minutely,hourly,daily,alerts" ;

            CallApi($"https://api.openweathermap.org/data/2.5/onecall?lat={cities[choice].Lat}&lon={cities[choice].Lon}&exclude={exclude}&appid={this.ApiKey}");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            OneCallRoot WeatherInfo = JsonSerializer.Deserialize<OneCallRoot>(this.ApiResponse);

            DateTime epochStart = new DateTime(1970, 1, 1);
            DateTime tmp1 = epochStart.AddSeconds(WeatherInfo.Current.Sunrise);
            DateTime tmp2 = epochStart.AddSeconds(WeatherInfo.Current.Sunset);

            SunRise = DateTime.SpecifyKind(tmp1, DateTimeKind.Utc);
            SunSet = DateTime.SpecifyKind(tmp2, DateTimeKind.Utc);

            Console.WriteLine($"SunRise = {SunRise}");
            Console.WriteLine($"SunSet  = {SunSet}");


            Console.WriteLine("<press [enter] to exit>");
            Console.ReadLine();

        }
        public void TemperatureNow() {

            int counter = 0 ;

            Console.Write("Enter city name: ");
            string sCity = Console.ReadLine();

            this.waiting = true ;

            CallApi($"http://api.openweathermap.org/geo/1.0/direct?q={sCity}&limit=10&appid={this.ApiKey}");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            List<GeoLocationRoot> cities = JsonSerializer.Deserialize<List<GeoLocationRoot>>(this.ApiResponse);
            Console.WriteLine("Choose the right country:");

            foreach (var city in cities) {
                Console.WriteLine($"{counter++} : Country = {city.Country}, State = {city.State}, Longitude = {city.Lon}, Latitude = {city.Lat}");
            }
            
            Console.Write("Enter your choice: ");
            var choice = Convert.ToInt32(Console.ReadLine());

            while (choice < 0 || choice > (counter-1))
            {
                Console.WriteLine("ERROR: Please enter a correct number!");
                Console.Write("Enter your choice : ");
                choice = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine($"Name = {cities[choice].Name} : Country = {cities[choice].Country}, State = {cities[choice].State}, Longitude = {cities[choice].Lon}, Latitude = {cities[choice].Lat}");

            this.waiting = true ;
            string exclude = "minutely,hourly,daily,alerts" ;

            CallApi($"https://api.openweathermap.org/data/2.5/onecall?lat={cities[choice].Lat}&lon={cities[choice].Lon}&exclude={exclude}&appid={this.ApiKey}&units=metric");

            Console.Write("Searching");
            while (this.waiting) {
                Console.Write(".");
            }

            Console.WriteLine("");
            /*
            Console.WriteLine("result: ");
            Console.WriteLine(this.ApiResponse);
            */

            OneCallRoot WeatherInfo = JsonSerializer.Deserialize<OneCallRoot>(this.ApiResponse);

            Console.WriteLine($"City = {sCity}, Temperature = {WeatherInfo.Current.Temp} °C");

            Console.WriteLine("<press [enter] to exit>");
            Console.ReadLine();

        }
        public void WindInCities() {

            // New York: Country = US, State = New York, Longitude = -74.0060152, Latitude = 40.7127281
            // Tokyo: Country = JP, State = , Longitude = 139.7594549, Latitude = 35.6828387
            // Paris: Country = FR, State = Ile-de-France, Longitude = 2.3514616, Latitude = 48.8566969

            int counter = 0 ;

            var speed = new List<double> {} ;
        

            var Cities = new List<City>
            {
                new City("new york",-74.0060152,40.7127281),
                new City("tokyo",-139.7594549,35.6828387),
                new City("paris",2.3514616,48.8566969)
            };

            string exclude = "minutely,hourly,daily,alerts" ;
            string units = "metric" ;
            string CityWindMin = "" ;
            double CityWindMinValue = -1.0 ;
            string CityWindMax = "" ;
            double CityWindMaxValue = -1.0 ;

            foreach (var city in Cities) {

                this.waiting = true ;

                CallApi($"https://api.openweathermap.org/data/2.5/onecall?lat={city.Lat}&lon={city.Lon}&exclude={exclude}&appid={this.ApiKey}&units={units}");

                Console.Write("Searching");
                while (this.waiting) {
                    //Console.Write(".");
                }

                Console.WriteLine("");
                /*
                Console.WriteLine("result: ");
                Console.WriteLine(this.ApiResponse);
                */

                OneCallRoot WeatherInfo = JsonSerializer.Deserialize<OneCallRoot>(this.ApiResponse);

                Console.WriteLine($"City = {city.Name}, Wind Speed = {WeatherInfo.Current.WindSpeed} Km/h");
                speed.Add(WeatherInfo.Current.WindSpeed);

                if (CityWindMin == "" || (WeatherInfo.Current.WindSpeed < CityWindMinValue)) {

                    CityWindMin = city.Name;
                    CityWindMinValue = WeatherInfo.Current.WindSpeed;

                }

                if (CityWindMax == "" || (WeatherInfo.Current.WindSpeed > CityWindMaxValue)) {

                    CityWindMax = city.Name;
                    CityWindMaxValue = WeatherInfo.Current.WindSpeed;

                }

            }

            Console.WriteLine($"Speed Max = {CityWindMaxValue} Km/h in City = {CityWindMax}");
            Console.WriteLine($"Speed Min = {CityWindMinValue} Km/h in City = {CityWindMin}");

            Console.WriteLine("<press [enter] to exit>");
            Console.ReadLine();

        }
    }
}
