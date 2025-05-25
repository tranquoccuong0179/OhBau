using System.Text.Json;

namespace OhBau.API.Middlewares
{
    public class VisitStatisticsMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly string _filePath;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public VisitStatisticsMiddleWare(RequestDelegate next)
        {
            _next = next;
            _filePath = "visitcount.json";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            await _semaphore.WaitAsync();
            try
            {
                Dictionary<string, List<string>> data;

                if (File.Exists(_filePath))
                {
                    string json = await File.ReadAllTextAsync(_filePath);
                    data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                           ?? new Dictionary<string, List<string>>();
                }
                else
                {
                    data = new Dictionary<string, List<string>>();
                }

                if (!data.ContainsKey(today))
                {
                    data[today] = new List<string>();
                }

                if (!data[today].Contains(ip))
                {
                    data[today].Add(ip);
                    string updatedJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    await File.WriteAllTextAsync(_filePath, updatedJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ghi log IP: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }

            await _next(context);
        }
    }
}
