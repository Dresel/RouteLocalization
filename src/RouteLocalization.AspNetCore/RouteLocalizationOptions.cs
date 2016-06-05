namespace RouteLocalization.AspNetCore
{
	using System.Collections.Generic;

	public class Configuration
	{
		public Configuration()
		{
			AddCultureAsRoutePrefix = false;

			AcceptedCultures = new HashSet<string>() { "en" };

			AddTranslationToSimiliarUrls = false;

			ValidateUrl = true;
			ValidateCulture = true;
		}

		// TODO: Validation
		public ISet<string> AcceptedCultures { get; set; }

		// TODO: Processing
		public bool AddCultureAsRoutePrefix { get; set; }

		// TODO: Processing. Remove?
		public bool AddTranslationToSimiliarUrls { get; set; }

		// TODO: Validation
		public bool ValidateCulture { get; set; }

		// TODO: Validation
		public bool ValidateUrl { get; set; }
	}
}