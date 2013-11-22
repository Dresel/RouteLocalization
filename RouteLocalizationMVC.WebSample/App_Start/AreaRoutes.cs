using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RouteLocalizationMVC.WebSample.Areas.Area.Controllers;

namespace RouteLocalizationMVC.WebSample.App_Start
{
	public static class AreaRoutes
	{
		public static void AddAreaRoutesTranslation(this Localization localization)
		{
			localization.ForCulture("de")
				.SetAreaPrefix("Area")
				.ForController<HomeController>()
				.ForAction(x => x.Index())
					.AddTranslation("Willkommen/{foo}")
				.ForAction(x => x.Book())
					.AddTranslation("Buch/{chapter}/{page}/{foo}");
		}
	}
}