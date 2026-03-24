using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Controllers;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests.Controllers;

[TestClass]
public class WeatherControllerTests
{
    private Mock<IWeatherService> _mockService;
    private WeatherController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IWeatherService>();
        var logger = new Mock<ILogger<WeatherController>>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        _controller = new WeatherController(_mockService.Object, logger.Object, cache);
    }

    [TestMethod]
    public async Task TP15_Success_Returns200WithData()
    {
        var vm = new WeatherViewModel
        {
            City = "Moscow",
            Current = new CurrentWeatherVm { Temp = 3.0, Description = "Cloudy" },
            Hourly = new List<HourlyItemVm>(),
            Daily = new List<DailyItemVm>()
        };
        _mockService.Setup(s => s.GetWeatherAsync()).ReturnsAsync(vm);

        var result = await _controller.Get();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(200, ok.StatusCode);

        var data = ok.Value as WeatherViewModel;
        Assert.IsNotNull(data);
        Assert.AreEqual("Moscow", data.City);
    }

    [TestMethod]
    public async Task TP16_HttpRequestException_Returns503()
    {
        _mockService.Setup(s => s.GetWeatherAsync())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var result = await _controller.Get();

        var obj = result as ObjectResult;
        Assert.IsNotNull(obj);
        Assert.AreEqual(503, obj.StatusCode);
    }

    [TestMethod]
    public async Task TP17_GenericException_Returns500()
    {
        _mockService.Setup(s => s.GetWeatherAsync())
            .ThrowsAsync(new InvalidOperationException("something broke"));

        var result = await _controller.Get();

        var obj = result as ObjectResult;
        Assert.IsNotNull(obj);
        Assert.AreEqual(500, obj.StatusCode);
    }

    [TestMethod]
    public async Task TP18_ErrorResponse_DoesNotLeakInternals()
    {
        _mockService.Setup(s => s.GetWeatherAsync())
            .ThrowsAsync(new Exception("secret_api_key_fa8b3df74d"));

        var result = await _controller.Get();

        var obj = result as ObjectResult;
        Assert.IsNotNull(obj);

        var body = Newtonsoft.Json.JsonConvert.SerializeObject(obj.Value);
        Assert.IsFalse(body.Contains("secret_api_key"), $"Error response should not contain internal details: {body}");
        Assert.IsFalse(body.Contains("fa8b3df74d"), $"Error response should not contain API key: {body}");
        Assert.IsTrue(body.Contains("Не удалось получить данные"));
    }

    [TestMethod]
    public async Task TP19_TaskCanceledException_Returns504()
    {
        _mockService.Setup(s => s.GetWeatherAsync())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        var result = await _controller.Get();

        var obj = result as ObjectResult;
        Assert.IsNotNull(obj);
        Assert.AreEqual(504, obj.StatusCode);
    }
}
