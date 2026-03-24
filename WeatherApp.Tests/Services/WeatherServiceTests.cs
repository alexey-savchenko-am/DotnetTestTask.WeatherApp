using System.Net;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using WeatherApp.Services;

namespace WeatherApp.Tests.Services;

[TestClass]
public class WeatherServiceTests
{
    private Mock<HttpMessageHandler> _handler;
    private WeatherService _service;
    private HttpRequestMessage _capturedRequest;

    [TestInitialize]
    public void Setup()
    {
        _handler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_handler.Object);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "WeatherApi:Key", "test-key-123" },
                { "WeatherApi:BaseUrl", "http://fake-api.test/v1" }
            })
            .Build();

        _service = new WeatherService(client, config);
    }

    private void MockResponse(HttpStatusCode status, string body)
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => _capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = status,
                Content = new StringContent(body)
            });
    }

    private string LoadTestJson() => File.ReadAllText("TestData/forecast-full.json");

    [TestMethod]
    public async Task TP04_ValidResponse_ReturnsCorrectViewModel()
    {
        MockResponse(HttpStatusCode.OK, LoadTestJson());

        var result = await _service.GetWeatherAsync();

        Assert.AreEqual("Moscow", result.City);
        Assert.AreEqual(3.0, result.Current.Temp);
        Assert.AreEqual(-1.2, result.Current.FeelsLike);
        Assert.AreEqual("Partly cloudy", result.Current.Description);
        Assert.AreEqual(65, result.Current.Humidity);
        Assert.AreEqual("W", result.Current.WindDir);
        Assert.IsTrue(result.Current.IsDay);
        Assert.AreEqual(3, result.Daily.Count);
        Assert.IsTrue(result.Hourly.Count > 0);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task TP05_Http500_ThrowsHttpRequestException()
    {
        MockResponse(HttpStatusCode.InternalServerError, "");
        await _service.GetWeatherAsync();
    }

    [TestMethod]
    [ExpectedException(typeof(TaskCanceledException))]
    public async Task TP06_Timeout_ThrowsTaskCanceledException()
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        await _service.GetWeatherAsync();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TP07_NullForecast_ThrowsException()
    {
        var json = @"{""location"":{""name"":""Moscow"",""localtime"":""2026-03-23 12:00""},""current"":{""temp_c"":3},""forecast"":null}";
        MockResponse(HttpStatusCode.OK, json);

        await _service.GetWeatherAsync();
    }

    [TestMethod]
    public async Task TP08_HourlyFilter_MiddleOfDay_FiltersTodayAndAddsTomorrow()
    {
        MockResponse(HttpStatusCode.OK, LoadTestJson());

        var result = await _service.GetWeatherAsync();

        Assert.AreEqual("14:00", result.Hourly[0].Time, "First hour should be 14:00 (filtered from today)");
        Assert.AreEqual(14, result.Hourly.Count, "10 hours from today (14-23) + 4 from tomorrow");
    }

    [TestMethod]
    public async Task TP09_HourlyFilter_EndOfDay_OnlyLastHoursToday()
    {
        var json = LoadTestJson().Replace("\"localtime\": \"2026-03-23 14:00\"", "\"localtime\": \"2026-03-23 23:00\"");
        MockResponse(HttpStatusCode.OK, json);

        var result = await _service.GetWeatherAsync();

        var allTimes = result.Hourly.Select(h => h.Time).ToList();
        Assert.IsTrue(allTimes[0] == "23:00", $"First hour should be 23:00, got {allTimes[0]}");
        Assert.IsTrue(result.Hourly.Count <= 5, $"Expected few hours at end of day + tomorrow, got {result.Hourly.Count}");
    }

    [TestMethod]
    public async Task TP10_HourlyFilter_StartOfDay_AllHoursIncluded()
    {
        var json = LoadTestJson().Replace("\"localtime\": \"2026-03-23 14:00\"", "\"localtime\": \"2026-03-23 00:30\"");
        MockResponse(HttpStatusCode.OK, json);

        var result = await _service.GetWeatherAsync();

        Assert.IsTrue(result.Hourly.Count >= 24, $"Expected all today hours + tomorrow, got {result.Hourly.Count}");
    }

    [TestMethod]
    public async Task TP11_IconUrl_ProtocolRelative_FixedToHttps()
    {
        MockResponse(HttpStatusCode.OK, LoadTestJson());

        var result = await _service.GetWeatherAsync();

        Assert.IsTrue(result.Current.Icon.StartsWith("https://"));
        Assert.IsTrue(result.Daily.All(d => d.Icon.StartsWith("https://")));
        Assert.IsTrue(result.Hourly.All(h => h.Icon.StartsWith("https://")));
    }

    [TestMethod]
    public async Task TP12_IconUrl_EmptyString_ReturnsAsIs()
    {
        var json = LoadTestJson().Replace("//cdn.weatherapi.com/weather/64x64/day/116.png", "");
        MockResponse(HttpStatusCode.OK, json);

        var result = await _service.GetWeatherAsync();

        Assert.AreEqual("", result.Current.Icon);
    }

    [TestMethod]
    public async Task TP13_RequestUrl_ContainsMoscowCoords()
    {
        MockResponse(HttpStatusCode.OK, LoadTestJson());

        await _service.GetWeatherAsync();

        Assert.IsNotNull(_capturedRequest);
        var url = _capturedRequest.RequestUri.ToString();
        Assert.IsTrue(url.Contains("q=55.7558,37.6173"), $"URL should contain Moscow coords: {url}");
    }

    [TestMethod]
    public async Task TP14_RequestUrl_ContainsApiKey()
    {
        MockResponse(HttpStatusCode.OK, LoadTestJson());

        await _service.GetWeatherAsync();

        var url = _capturedRequest.RequestUri.ToString();
        Assert.IsTrue(url.Contains("key=test-key-123"), $"URL should contain API key: {url}");
    }
}
