namespace POC_PostgreSQL.Models
{
    public class WeatherForecastByLocation
    {
        public WeatherForecast[]? WeatherForecasts { get; set; }

        public Location? Location { get; set; }
    }
}
