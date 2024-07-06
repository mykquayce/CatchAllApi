namespace Microsoft.Extensions.Logging;

public class CollectionLogger(string categoryName, ICollection<string> entries) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
	public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		var entry = string.Join(Environment.NewLine,
			string.Format("{0}: {1}[{2}]", logLevel.ToString("g")[..4].ToLowerInvariant(), categoryName, eventId),
			formatter(state, exception));
		entries.Add(entry);
	}
}
