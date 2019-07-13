using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherService.Models
{
    public class CityDetail
    {
        public long id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public Coord coord { get; set; }
    }

    public class Coord
    {
        public decimal lon { get; set; }
        public decimal lat { get; set; }
    }
}