#if ASPNETWEBAPI
namespace RouteLocalization.Http.Setup
#else
namespace RouteLocalization.Mvc.Setup
#endif
{
	using System.Collections.Generic;

#if ASPNETWEBAPI
	using System.Web.Http.Routing;
#else
	using System.Web.Mvc.Routing;
#endif

	public class Configuration
	{
		private List<RouteEntry> localizationCollectionRoutes;

		public Configuration()
		{
			DefaultCulture = "en";

			AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;
			AddCultureAsRoutePrefix = false;

			AcceptedCultures = new HashSet<string>() { "en" };

			ValidateUrl = true;
			ValidateRouteArea = true;
			ValidateRoutePrefix = true;
			ValidateCulture = true;

			AddTranslationToSimiliarUrls = false;
		}

		public HashSet<string> AcceptedCultures { get; set; }

		public bool AddCultureAsRoutePrefix { get; set; }

		public bool AddTranslationToSimiliarUrls { get; set; }

		public AttributeRouteProcessing AttributeRouteProcessing { get; set; }

		public string DefaultCulture { get; set; }

		public List<RouteEntry> LocalizationCollectionRoutes
		{
			get
			{
				if (this.localizationCollectionRoutes == null)
				{
					this.localizationCollectionRoutes = Localization.LocalizationDirectRouteProvider.LocalizationCollectionRoutes;
				}

				return this.localizationCollectionRoutes;
			}

			set
			{
				this.localizationCollectionRoutes = value;
			}
		}

		public bool ValidateCulture { get; set; }

		public bool ValidateRouteArea { get; set; }

		public bool ValidateRoutePrefix { get; set; }

		public bool ValidateUrl { get; set; }
	}
}