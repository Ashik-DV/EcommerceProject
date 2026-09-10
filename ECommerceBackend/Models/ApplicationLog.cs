using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.Models;

public class ApplicationLog
{
[Key]
public int Id { get; set; }

public string Level { get; set; } = "Information";

public string HttpMethod { get; set; } = string.Empty;

public string RequestPath { get; set; } = string.Empty;

public string QueryString { get; set; } = string.Empty;

public DateTime RequestStartTime { get; set; }

public int ResponseStatusCode { get; set; }

public long ExecutionDurationMs { get; set; }

public string ClientIp { get; set; } = string.Empty;

public string CorrelationId { get; set; } = string.Empty;

public string Message { get; set; } = string.Empty;

}