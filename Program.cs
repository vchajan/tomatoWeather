using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DotNetEnv;
class Program
{


    static readonly string lat = "48.1486"; // Bratislava latitude
    static readonly string lon = "17.1077"; // Bratislava longitude

    static async Task Main()
    {
        Env.Load(); // Load API key from .env
        string apiKey = Env.GetString("API_KEY");
        Console.WriteLine("Loaded API KEY: " + (string.IsNullOrWhiteSpace(apiKey) ? "EMPTY" : "✔️ OK"));
        using var db = new ForecastContext();
        db.Database.EnsureCreated();

        Console.WriteLine("Checking weather forecast for tomato planting...");

        using HttpClient client = new HttpClient();
        string url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=hourly,minutely,current,alerts&units=metric&appid={apiKey}";
        Console.WriteLine($"Fetching data from: {url}");

        var response = await client.GetStringAsync(url);
        JObject data = JObject.Parse(response);
        var daily = data["daily"];

        int goodDays = 0;
        DateTime now = DateTime.Now;

        for (int i = 0; i < 4; i++)
        {
            var day = daily[i];
            double dayTemp = day["temp"]["day"].Value<double>();
            double nightTemp = day["temp"]["night"].Value<double>();
            double windSpeed = day["wind_speed"]?.Value<double>() ?? 0;
            double rain = day["rain"]?.Value<double>() ?? 0;
            long unixTime = day["dt"].Value<long>();
            DateTime forecastDate = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;

            bool conditionsMet = dayTemp > 15 && nightTemp > 10 && windSpeed < 7 && rain < 1;
            if (conditionsMet)
                goodDays++;

            db.ForecastEntries.Add(new ForecastEntry
            {
                CheckedAt = now,
                ForecastDate = forecastDate,
                DayTemp = dayTemp,
                NightTemp = nightTemp,
                WindSpeed = windSpeed,
                Rain = rain,
                ConditionsMet = conditionsMet
            });

            Console.WriteLine($"Day {i + 1} ({forecastDate:yyyy-MM-dd}): Day {dayTemp}°C, Night {nightTemp}°C, Wind {windSpeed} m/s, Rain {rain} mm => {(conditionsMet ? "✅ Good" : "❌ Bad")}");
        }

        await db.SaveChangesAsync();

        Console.WriteLine();

        if (goodDays == 4)
            Console.WriteLine("✅ You can safely set your tomatoes! 🍅");
        else
            Console.WriteLine("❌ Not ideal yet. Wait a few more days.");
    }

}
