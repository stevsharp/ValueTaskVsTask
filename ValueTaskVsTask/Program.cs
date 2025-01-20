using System.Diagnostics;
using System.Text.Json;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://localhost:7023/WeatherForecast"; // Replace with your API URL

    static async Task Main(string[] args)
    {
        const int numberOfCalls = 1_000;

        Console.WriteLine("Starting Task test...");
        await MeasurePerformance(CallApiUsingTask, numberOfCalls);

        Console.WriteLine("Starting ValueTask test...");
        await MeasurePerformance(CallApiUsingValueTask, numberOfCalls);

        Console.ReadLine();
    }

    static async Task<IEnumerable<WeatherForecast>> CallApiUsingTask()
    {
        var response = await _httpClient.GetAsync(ApiUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
    }

    // HTTP Call using ValueTask
    static async ValueTask<IEnumerable<WeatherForecast>> CallApiUsingValueTask()
    {
        var response = await _httpClient.GetAsync(ApiUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
    }

    static async Task MeasurePerformance(Func<Task<IEnumerable<WeatherForecast>>> asyncFunction, int numberOfCalls)
    {
        var stopwatch = Stopwatch.StartNew();
        long initialMemory = GC.GetTotalMemory(forceFullCollection: true);

        for (int i = 0; i < numberOfCalls; i++)
        {
            await asyncFunction();
        }

        stopwatch.Stop();
        long finalMemory = GC.GetTotalMemory(forceFullCollection: true);

        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Memory allocated: {finalMemory - initialMemory} bytes");
    }

    static async Task MeasurePerformance(Func<ValueTask<IEnumerable<WeatherForecast>>> asyncFunction, int numberOfCalls)
    {
        var stopwatch = Stopwatch.StartNew();
        long initialMemory = GC.GetTotalMemory(forceFullCollection: true);

        for (int i = 0; i < numberOfCalls; i++)
        {
            await asyncFunction();
        }

        stopwatch.Stop();
        long finalMemory = GC.GetTotalMemory(forceFullCollection: true);

        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Memory allocated: {finalMemory - initialMemory} bytes");
    }
}

public record WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
}
