namespace RouteLocalizationMVC.Setup
{
	using System.Collections.Generic;

	public static class Configuration
	{
		static Configuration()
		{
			DefaultCulture = "en";

			ApplyDefaultCultureToRootRoute = true;
			AddCultureAsRoutePrefix = true;

			AcceptedCultures = new HashSet<string>() { "en" };

			ValidateURL = true;
			ValidateRouteArea = true;
			ValidateRoutePrefix = true;
			ValidateCulture = true;
		}

		public static HashSet<string> AcceptedCultures { get; set; }

		public static bool AddCultureAsRoutePrefix { get; set; }

		public static bool ApplyDefaultCultureToRootRoute { get; set; }

		public static string DefaultCulture { get; set; }

		public static bool ValidateCulture { get; set; }

		public static bool ValidateRouteArea { get; set; }

		public static bool ValidateRoutePrefix { get; set; }

		public static bool ValidateURL { get; set; }
	}
}