using System.Text;
using ECommerceBackend.Models;

namespace ECommerceBackend.Logging;

public class FileLogger : IApplicationLogger
{
private readonly string _logFilePath;
private readonly SemaphoreSlim _semaphore = new(1, 1);

public FileLogger(IWebHostEnvironment environment)
{
    var logsDirectory = Path.Combine(
        environment.ContentRootPath,
        "Logs");

    Directory.CreateDirectory(logsDirectory);

    _logFilePath = Path.Combine(
        logsDirectory,
        "application.log");
}

public async Task LogAsync(ApplicationLog log)
{
    var logText = new StringBuilder();

    logText.AppendLine($"[{log.RequestStartTime:yyyy-MM-dd HH:mm:ss}]");
    logText.AppendLine($"Level: {log.Level}");
    logText.AppendLine($"CorrelationId: {log.CorrelationId}");
    logText.AppendLine(
        $"{log.HttpMethod} {log.RequestPath}{log.QueryString}");
    logText.AppendLine($"Status: {log.ResponseStatusCode}");
    logText.AppendLine($"Duration: {log.ExecutionDurationMs}ms");
    logText.AppendLine($"Client IP: {log.ClientIp}");
    logText.AppendLine($"Message: {log.Message}");
    logText.AppendLine(new string('-', 80));

    await _semaphore.WaitAsync();

    try
    {
        await File.AppendAllTextAsync(
            _logFilePath,
            logText.ToString());
    }
    finally
    {
        _semaphore.Release();
    }
}

}