namespace RouteLocalization.AspNetCore
{
	using System;
	using Microsoft.Extensions.Logging;

	// See https://github.com/aspnet/Logging/blob/dev/src/Microsoft.Extensions.Logging.Abstractions/NullLoggerOfT.cs
	public class NullLogger<T> : ILogger<T>
	{
		public static readonly NullLogger<T> Instance = new NullLogger<T>();

		public IDisposable BeginScope<TState>(TState state)
		{
			return NullDisposable.Instance;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return false;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
		}

		private class NullDisposable : IDisposable
		{
			public static readonly NullDisposable Instance = new NullDisposable();

			public void Dispose()
			{
			}
		}
	}
}