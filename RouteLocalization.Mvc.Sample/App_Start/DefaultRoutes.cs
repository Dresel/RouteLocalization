namespace RouteLocalization.Mvc.Sample
{
	using RouteLocalization.Mvc.Sample.Controllers;

	public static class DefaultRoutes
	{
		public static void AddDefaultRoutesTranslation(this Localization localization)
		{
			localization.ForCulture("de")
				.ForController<HomeController>()
				.ForAction(x => x.Index())
				.AddTranslation("Willkommen");
				////.ForAction(x => x.Book())
				////.AddTranslation("Buch/{chapter}/{page}");

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