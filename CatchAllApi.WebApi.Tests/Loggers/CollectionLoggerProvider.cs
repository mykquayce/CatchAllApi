namespace Microsoft.Extensions.Logging;

public sealed class CollectionLoggerProvider : ILoggerProvider
{
	public ILogger CreateLogger(string categoryName) => new CollectionLogger(categoryName, Entries);
	public ICollection<string> Entries { get; } = [];
	public void Dispose() { }
}
