using Moq;
using Moq.Protected;

using System.Net;
using System.Text.Json;

public class ProgramTests
{

    [Fact]
    public async Task CallApiUsingTask_ReturnsWeatherForecasts()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new List<WeatherForecast>
                {
                    new WeatherForecast { Date = new DateOnly(2023, 1, 1), TemperatureC = 25, Summary = "Sunny" }
                }))
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        HttpService.SetHttpClient(httpClient);

        // Act
        var result = await HttpService.CallApiUsingTask();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(25, result.First().TemperatureC);
    }

    [Fact]
    public async Task CallApiUsingValueTask_ReturnsWeatherForecasts()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new List<WeatherForecast>
                {
                    new WeatherForecast { Date = new DateOnly(2023, 1, 1), TemperatureC = 25, Summary = "Sunny" }
                }))
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        HttpService.SetHttpClient(httpClient);

        // Act
        var result = await HttpService.CallApiUsingValueTask();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(25, result.First().TemperatureC);
    }
}

public static class HttpService
{
    private static HttpClient _httpClient = new HttpClient();

    public static void SetHttpClient(HttpClient client)
    {
        _httpClient = client;
    }

    public static async Task<IEnumerable<WeatherForecast>> CallApiUsingTask()
    {
        var response = await _httpClient.GetAsync("https://localhost:7023/WeatherForecast");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
    }

    public static async ValueTask<IEnumerable<WeatherForecast>> CallApiUsingValueTask()
    {
        var response = await _httpClient.GetAsync("https://localhost:7023/WeatherForecast");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
    }
}

public record WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
}
