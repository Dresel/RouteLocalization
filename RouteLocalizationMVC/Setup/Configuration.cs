namespace RouteLocalizationMVC.Setup
{
	using System.Collections.Generic;

	public class Configuration
	{
		public Configuration()
		{
			DefaultCulture = "en";

			ApplyDefaultCultureToRootRoute = true;
			AddCultureAsRoutePrefix = false;

			AcceptedCultures = new HashSet<string>() { "en" };

			ValidateURL = true;
			ValidateRouteArea = true;
			ValidateRoutePrefix = true;
			ValidateCulture = true;
		}

		public HashSet<string> AcceptedCultures { get; set; }

		public bool AddCultureAsRoutePrefix { get; set; }

		public bool ApplyDefaultCultureToRootRoute { get; set; }

		public string DefaultCulture { get; set; }

		public bool ValidateCulture { get; set; }

		public bool ValidateRouteArea { get; set; }

		public bool ValidateRoutePrefix { get; set; }

		public bool ValidateURL { get; set; }
	}
}