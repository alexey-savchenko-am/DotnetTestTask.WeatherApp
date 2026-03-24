using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeatherApp.Services;

namespace WeatherApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "weather_data";

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger, IMemoryCache cache)
    {
        _weatherService = weatherService;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var result = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _weatherService.GetWeatherAsync();
            });
            return Ok(result);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Weather API timeout");
            return StatusCode(504, new { error = "Сервис погоды не ответил вовремя" });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Weather API недоступен");
            return StatusCode(503, new { error = "Сервис погоды временно недоступен" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении погоды");
            return StatusCode(500, new { error = "Не удалось получить данные о погоде" });
        }
    }
}
