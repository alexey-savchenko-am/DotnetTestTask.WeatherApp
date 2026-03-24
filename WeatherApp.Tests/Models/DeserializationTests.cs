using Newtonsoft.Json;
using WeatherApp.Models;

namespace WeatherApp.Tests.Models;

[TestClass]
public class DeserializationTests
{
    private string _fullJson;

    [TestInitialize]
    public void Setup()
    {
        _fullJson = File.ReadAllText("TestData/forecast-full.json");
    }

    [TestMethod]
    public void TP01_FullJson_AllFieldsMapped()
    {
        var result = JsonConvert.DeserializeObject<ForecastResponse>(_fullJson);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Location);
        Assert.IsNotNull(result.Current);
        Assert.IsNotNull(result.Forecast);

        Assert.AreEqual("Moscow", result.Location.Name);
        Assert.AreEqual(55.75, result.Location.Lat);
        Assert.AreEqual("Europe/Moscow", result.Location.TzId);

        Assert.AreEqual(3.0, result.Current.TempC);
        Assert.AreEqual(-1.2, result.Current.FeelslikeC);
        Assert.AreEqual(15.1, result.Current.WindKph);
        Assert.AreEqual("W", result.Current.WindDir);
        Assert.AreEqual(1013.0, result.Current.PressureMb);
        Assert.AreEqual(65, result.Current.Humidity);
        Assert.AreEqual(1, result.Current.IsDay);
        Assert.AreEqual("Partly cloudy", result.Current.Condition.Text);

        Assert.AreEqual(3, result.Forecast.Forecastday.Count);
        Assert.AreEqual(24, result.Forecast.Forecastday[0].Hour.Count);
    }

    [TestMethod]
    public void TP02_NullForecast_DeserializesWithoutException()
    {
        var json = @"{""location"":{""name"":""Moscow"",""localtime"":""2026-03-23 12:00""},""current"":{""temp_c"":3,""condition"":{""text"":""Clear"",""icon"":"""",""code"":1000}},""forecast"":null}";

        var result = JsonConvert.DeserializeObject<ForecastResponse>(json);

        Assert.IsNotNull(result);
        Assert.IsNull(result.Forecast);
    }

    [TestMethod]
    public void TP03_SnakeCaseProperties_MappedCorrectly()
    {
        var result = JsonConvert.DeserializeObject<ForecastResponse>(_fullJson);

        Assert.AreEqual(-1.2, result.Current.FeelslikeC);
        Assert.AreEqual(15.1, result.Current.WindKph);
        Assert.AreEqual("W", result.Current.WindDir);
        Assert.AreEqual(1, result.Current.IsDay);
        Assert.AreEqual(1013.0, result.Current.PressureMb);

        var hour = result.Forecast.Forecastday[0].Hour[14];
        Assert.AreEqual(5, hour.ChanceOfRain);
        Assert.AreEqual(1, hour.IsDay);
        Assert.AreEqual(15.0, hour.WindKph);

        Assert.AreEqual(5.0, result.Forecast.Forecastday[0].Day.MaxtempC);
        Assert.AreEqual(-2.0, result.Forecast.Forecastday[0].Day.MintempC);
    }
}
