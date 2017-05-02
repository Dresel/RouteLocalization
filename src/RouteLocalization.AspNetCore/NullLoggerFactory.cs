namespace RouteLocalization.AspNetCore
{
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Logging.Abstractions;

	// See https://github.com/aspnet/Logging/blob/dev/src/Microsoft.Extensions.Logging.Abstractions/NullLoggerFactory.cs
	public class NullLoggerFactory : ILoggerFactory
	{
		public static readonly NullLoggerFactory Instance = new NullLoggerFactory();

		public void AddProvider(ILoggerProvider provider)
		{
		}

		public ILogger CreateLogger(string name)
		{
			return NullLogger.Instance;
		}

		public void Dispose()
		{
		}
	}
}