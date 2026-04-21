using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenAI_With_AgentFramework.Tools
{
    public class WeatherTool
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherTool(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["WeatherApiKey"];
        }

        
        [Description("Gets the current weather for a specified location.")]
        public async Task<string> GetWeatherAsync([Description("The location for which to get the weather.")] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return "Location is required.";

            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return $"Could not fetch weather for '{location}'.";

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);

                var root = doc.RootElement;

                var temp = root.GetProperty("main").GetProperty("temp").GetDecimal();
                var description = root.GetProperty("weather")[0].GetProperty("description").GetString();
                var city = root.GetProperty("name").GetString();

                return $"Weather in {city}: {temp}°C, {description}";
            }
            catch (Exception ex)
            {
                return $"Error fetching weather: {ex.Message}";
            }
        }
    }
}
