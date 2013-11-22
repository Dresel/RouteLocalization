using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RouteLocalizationMVC.WebSample.Controllers;

namespace RouteLocalizationMVC.WebSample.App_Start
{
	public static class DefaultRoutes
	{
		public static void AddDefaultRoutesTranslation(this Localization localization)
		{
			localization.ForCulture("de")
				.ForController<HomeController>()
				.ForAction(x => x.Index())
					.AddTranslation("Willkommen")
				.ForAction(x => x.Book())
					.AddTranslation("Buch/{chapter}/{page}");

			localization.ForCulture("de")
				.ForController<HomeWithRoutePrefixAttributeController>()
				.SetRoutePrefix("RoutePrefixDE")
					.ForAction(x => x.Index())
						.AddTranslation("Willkommen")
					.ForAction(x => x.Book())
						.AddTranslation("Buch/{chapter}/{page}");

			localization.ForCulture("de")
				.SetAreaPrefix("AreaPrefixDE")
				.ForController<HomeWithRouteAreaAttributeController>()
					.ForAction(x => x.Index())
						.AddTranslation("Willkommen")
					.ForAction(x => x.Book())
						.AddTranslation("Buch/{chapter}/{page}");
		}
	}
}