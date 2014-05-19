namespace RouteLocalization.Http.Extensions
{
	using System;
	using System.Web.Http;

	public static class HttpConfigurationExtensions
	{
		public static void ContinueAfterPreviousInitialization(this HttpConfiguration configuration,
			Action<HttpConfiguration> httpConfigurationAction)
		{
			Action<HttpConfiguration> previousInitializer = configuration.Initializer;
			configuration.Initializer = config =>
			{
				// Chain to the previous initializer hook. Do this before we access the config since
				// initialization may make last minute changes to the configuration.
				previousInitializer(config);

				httpConfigurationAction.Invoke(configuration);
			};
		}
	}
}