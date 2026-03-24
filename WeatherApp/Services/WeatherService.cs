using System.Globalization;
using Newtonsoft.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    private const string MoscowCoords = "55.7558,37.6173";

    public WeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["WeatherApi:Key"];
        _baseUrl = config["WeatherApi:BaseUrl"];
    }

    public async Task<WeatherViewModel> GetWeatherAsync()
    {
        var url = $"{_baseUrl}/forecast.json?key={_apiKey}&q={MoscowCoords}&days=3";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ForecastResponse>(json);

        if (data?.Forecast?.Forecastday == null)
            throw new InvalidOperationException("Невалидный ответ от API");

        return BuildViewModel(data);
    }

    private WeatherViewModel BuildViewModel(ForecastResponse data)
    {
        var now = DateTime.Parse(data.Location.Localtime, CultureInfo.InvariantCulture);

        var hourly = new List<HourlyItemVm>();

        if (data.Forecast.Forecastday.Count > 0)
        {
            var todayHours = data.Forecast.Forecastday[0].Hour
                .Where(h => DateTime.Parse(h.Time, CultureInfo.InvariantCulture) >= now.AddMinutes(-30))
                .ToList();
            hourly.AddRange(todayHours.Select(MapHour));
        }

        if (data.Forecast.Forecastday.Count > 1)
        {
            hourly.AddRange(data.Forecast.Forecastday[1].Hour.Select(MapHour));
        }

        var daily = data.Forecast.Forecastday.Select(d => new DailyItemVm
        {
            Date = d.Date,
            MaxTemp = d.Day.MaxtempC,
            MinTemp = d.Day.MintempC,
            Description = d.Day.Condition.Text,
            Icon = FixIconUrl(d.Day.Condition.Icon)
        }).ToList();

        return new WeatherViewModel
        {
            City = data.Location.Name,
            Current = new CurrentWeatherVm
            {
                Temp = data.Current.TempC,
                FeelsLike = data.Current.FeelslikeC,
                Description = data.Current.Condition.Text,
                Icon = FixIconUrl(data.Current.Condition.Icon),
                WindKph = data.Current.WindKph,
                WindDir = data.Current.WindDir,
                Humidity = data.Current.Humidity,
                Pressure = data.Current.PressureMb,
                IsDay = data.Current.IsDay == 1
            },
            Hourly = hourly,
            Daily = daily
        };
    }

    private HourlyItemVm MapHour(HourData h) => new()
    {
        Time = DateTime.Parse(h.Time, CultureInfo.InvariantCulture).ToString("HH:mm"),
        Temp = h.TempC,
        Icon = FixIconUrl(h.Condition.Icon),
        Description = h.Condition.Text,
        ChanceOfRain = h.ChanceOfRain
    };

    private string FixIconUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return url;
        return url.StartsWith("//") ? "https:" + url : url;
    }
}
