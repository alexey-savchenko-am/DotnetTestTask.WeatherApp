namespace WeatherApp.Models;

public class WeatherViewModel
{
    public CurrentWeatherVm Current { get; set; }
    public List<HourlyItemVm> Hourly { get; set; }
    public List<DailyItemVm> Daily { get; set; }
    public string City { get; set; }
}

public class CurrentWeatherVm
{
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public double WindKph { get; set; }
    public string WindDir { get; set; }
    public int Humidity { get; set; }
    public double Pressure { get; set; }
    public bool IsDay { get; set; }
}

public class HourlyItemVm
{
    public string Time { get; set; }
    public double Temp { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public int ChanceOfRain { get; set; }
}

public class DailyItemVm
{
    public string Date { get; set; }
    public double MaxTemp { get; set; }
    public double MinTemp { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}
