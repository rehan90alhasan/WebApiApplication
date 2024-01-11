using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace WebApiApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index).ToString(),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetDb",Name = "GetDbWeatherForecast")]
        public List<WeatherForecast> GetFromDb()
        {
            try
            {
                var mySqlConnection = new MySqlConnection("server=webapi-mysql;user=root;password=password;database=mysql;Convert Zero Datetime=True");
                mySqlConnection.Open();
                var seedSqlCommand = new MySqlCommand(@$"CREATE TABLE IF NOT EXISTS Forcast (ForcastId int NOT NULL AUTO_INCREMENT, TemperatureC int, Summary varchar(255), RecDate Date, PRIMARY KEY (ForcastId));
                                                    REPLACE INTO Forcast (TemperatureC, Summary, RecDate)
VALUES ({Random.Shared.Next(-20, 55)}, '{Summaries[Random.Shared.Next(Summaries.Length)]}', '{DateOnly.FromDateTime(DateTime.Now.AddDays(1)).ToString("yyyy-MM-dd")}');
REPLACE INTO Forcast (TemperatureC, Summary, RecDate)
VALUES ({Random.Shared.Next(-20, 55)}, '{Summaries[Random.Shared.Next(Summaries.Length)]}', '{DateOnly.FromDateTime(DateTime.Now.AddDays(2)).ToString("yyyy-MM-dd")}');
REPLACE INTO Forcast (TemperatureC, Summary, RecDate)
VALUES ({Random.Shared.Next(-20, 55)}, '{Summaries[Random.Shared.Next(Summaries.Length)]}', '{DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString("yyyy-MM-dd")}');
REPLACE INTO Forcast (TemperatureC, Summary, RecDate)
VALUES ({Random.Shared.Next(-20, 55)}, '{Summaries[Random.Shared.Next(Summaries.Length)]}', '{DateOnly.FromDateTime(DateTime.Now.AddDays(4)).ToString("yyyy-MM-dd")}');
REPLACE INTO Forcast (TemperatureC, Summary, RecDate)
VALUES ({Random.Shared.Next(-20, 55)}, '{Summaries[Random.Shared.Next(Summaries.Length)]}', '{DateOnly.FromDateTime(DateTime.Now.AddDays(5)).ToString("yyyy-MM-dd")}');", mySqlConnection);
                seedSqlCommand.ExecuteNonQuery();
                var mySqlCommand = new MySqlCommand("SELECT * FROM Forcast;", mySqlConnection);
                var mySqlReader = mySqlCommand.ExecuteReader();
                var forecasts = new List<WeatherForecast>();

                while (mySqlReader.Read())
                {

                    forecasts.Add(new WeatherForecast
                    {
                        Date = mySqlReader.GetDateTime("RecDate").ToShortDateString(),
                        Summary = mySqlReader.GetString("Summary"),
                        TemperatureC = mySqlReader.GetInt32("TemperatureC")

                    });
                }

                return forecasts;
            }
            catch (Exception ex)
            {

                return new List<WeatherForecast>() { new WeatherForecast
                {
                    Summary = ex.Message,
                    Date = DateTime.Now.ToShortDateString(),
                    TemperatureC = 60
                } };
            }
            
        }
    }
}
