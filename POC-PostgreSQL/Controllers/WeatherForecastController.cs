using Microsoft.AspNetCore.Mvc;

using Npgsql;

using POC_PostgreSQL.Models;

namespace POC_PostgreSQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecastByLocation> Get()
        {
            // Always store connection strings securely. 
            var connectionString = "Server=172.17.0.3; Port=5432; Database=main; User Id=main; Password=password;";
            var result = new List<WeatherForecastByLocation>();

            // Best practice is to scope the NpgsqlConnection to a "using" block
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                // Connect to the database
                connection.Open();

                logger.LogInformation("Getting locations data");

                // Read rows
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Locations WHERE Active = true", connection);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new WeatherForecastByLocation
                        {
                            Location = new Location
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = Convert.ToString(reader["name"]),
                                Active = Convert.ToBoolean(reader["active"])
                            },
                            WeatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                            {
                                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                TemperatureC = Random.Shared.Next(-20, 55),
                                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                            }).ToArray()
                        });
                    }
                }
            }

            return result;
        }
    }
}
