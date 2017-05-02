namespace RouteLocalization.AspNetCore.Test
{
	using Microsoft.Extensions.Options;

	public class TestOptionsManager<TOptions> : IOptions<TOptions> where TOptions : class, new()
	{
		public TestOptionsManager() : this(new TOptions())
		{
		}

		public TestOptionsManager(TOptions value)
		{
			Value = value;
		}

		public TOptions Value { get; }
	}
}