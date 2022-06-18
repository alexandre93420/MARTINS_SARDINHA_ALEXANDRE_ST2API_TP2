using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meteo
{
    public class City
    {
        public string Name { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }

        public City()
        {
            this.Name = "";
            this.Lon = 0.0;
            this.Lat = 0.0;
        }

        public City(string sName, double dLon, double dLat)
        {
            this.Name = sName;
            this.Lon = dLon;
            this.Lat = dLat;
        }

    }
}
