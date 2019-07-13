using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WeatherService.Models;

namespace WeatherService.Controllers
{
    public class WeatherController : Controller
    {
        static List<CityDetail> cityList = null;
        public WeatherController()
        {
            if (cityList == null)
            {
                cityList = GetAllCityIds();
            }
        }

        // GET: Weather
        [HttpGet]
        public ActionResult GenerateWeatherReport()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult GenerateWeatherReport(HttpPostedFileBase file)
        {
            var supportedTypes = new[] { "csv" };
            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                TempData["ErrorMessage"] = "File Extension Is InValid - Only .csv file allowed";
            }
            else
            {
                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes(file.ContentLength);
                string result = System.Text.Encoding.UTF8.GetString(binData);
                string[] cities = result.Split(',');

                var givenCityDetails = from c1 in cityList
                                       join c2 in cities
                                       on c1.name equals c2
                                       select c1;

                foreach (var city in givenCityDetails)
                {
                    GenerateWeatherReportForToday(city);
                }

                TempData["SuccessMessage"] = "Weather report has been generated for the uploaded cities.";
            }

            return RedirectToAction("GenerateWeatherReport");
        }

        private List<CityDetail> GetAllCityIds()
        {
            FileStream fileStream = new FileStream(String.Format("{0}\\city.list.json", AppDomain.CurrentDomain.BaseDirectory), FileMode.Open, FileAccess.Read);
            BinaryReader b = new BinaryReader(fileStream);
            byte[] binData = b.ReadBytes((int)fileStream.Length);
            string cityListString = System.Text.Encoding.UTF8.GetString(binData);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            List<CityDetail> cityDetails = serializer.Deserialize<List<CityDetail>>(cityListString);
            return cityDetails;
        }

        private void GenerateWeatherReportForToday(CityDetail city)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://api.openweathermap.org")
            };

            string appId = "aa69195559bd4f88d79f9aadeb77a8f6";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("/data/2.5/weather?id={0}&APPID={1}", city.id, appId));
            HttpResponseMessage httpResponseMessage = client.SendAsync(request).Result;
            string output = httpResponseMessage.Content.ReadAsStringAsync().Result;

            var path = String.Format("{0}WeatherReport\\city_{1},{2}_{3}.txt", AppDomain.CurrentDomain.BaseDirectory, city.name, city.country, DateTime.Today.ToString("yyyyMMdd"));
            System.IO.File.WriteAllText(path, output);
        }
    }
}