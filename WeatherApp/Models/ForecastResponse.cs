using Newtonsoft.Json;

namespace WeatherApp.Models;

public class ForecastResponse
{
    public LocationData Location { get; set; }
    public CurrentData Current { get; set; }
    public ForecastData Forecast { get; set; }
}

public class LocationData
{
    public string Name { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }

    [JsonProperty("tz_id")]
    public string TzId { get; set; }

    public string Localtime { get; set; }
}

public class CurrentData
{
    [JsonProperty("temp_c")]
    public double TempC { get; set; }

    [JsonProperty("feelslike_c")]
    public double FeelslikeC { get; set; }

    [JsonProperty("wind_kph")]
    public double WindKph { get; set; }

    [JsonProperty("wind_dir")]
    public string WindDir { get; set; }

    [JsonProperty("pressure_mb")]
    public double PressureMb { get; set; }

    public int Humidity { get; set; }

    [JsonProperty("is_day")]
    public int IsDay { get; set; }

    public double Uv { get; set; }

    public ConditionData Condition { get; set; }
}

public class ConditionData
{
    public string Text { get; set; }
    public string Icon { get; set; }
    public int Code { get; set; }
}

public class ForecastData
{
    public List<ForecastDayData> Forecastday { get; set; }
}

public class ForecastDayData
{
    public string Date { get; set; }
    public DayData Day { get; set; }
    public List<HourData> Hour { get; set; }
}

public class DayData
{
    [JsonProperty("maxtemp_c")]
    public double MaxtempC { get; set; }

    [JsonProperty("mintemp_c")]
    public double MintempC { get; set; }

    public ConditionData Condition { get; set; }
}

public class HourData
{
    public string Time { get; set; }

    [JsonProperty("temp_c")]
    public double TempC { get; set; }

    [JsonProperty("wind_kph")]
    public double WindKph { get; set; }

    [JsonProperty("chance_of_rain")]
    public int ChanceOfRain { get; set; }

    [JsonProperty("is_day")]
    public int IsDay { get; set; }

    public ConditionData Condition { get; set; }
}
