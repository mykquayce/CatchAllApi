var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Map("/{**catchAll}", async (ILogger<Program> logger, HttpRequest request, string? catchAll) => 
{
	logger.LogInformation("{time:O}", DateTime.UtcNow);
	logger.LogInformation("{method} {path}", request.Method, request.Path);
	foreach (var (key, values) in request.Headers) { logger.LogInformation("Header: {key} = {values}", key, values); }
	if (request.ContentType != null)
	{
		foreach (var (key, values) in request.Form) { logger.LogInformation("Form: {key} = {values}", key, values); }
	}
	using var reader = new StreamReader(request.Body);
	var body = await reader.ReadToEndAsync();
	logger.LogInformation("Body: {body}", body);
	return "Hello World!";
});

app.Run();

public partial class Program { }
