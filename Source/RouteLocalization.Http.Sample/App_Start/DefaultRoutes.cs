namespace RouteLocalization.Http.Sample
{
	using RouteLocalization.Http;
	using RouteLocalization.Http.Sample.Controllers;

	public static class DefaultRoutes
	{
		public static void AddDefaultRoutesTranslation(this Localization localization)
		{
			localization.ForCulture("de")
				.ForController<HomeController>()
				.ForAction(x => x.Index(0))
				.AddTranslation("Willkommen")
				.ForAction(x => x.Book(0, 0))
				.AddTranslation("Buch/{chapter}/{page}");

			localization.ForCulture("de")
				.ForController<HomeWithRoutePrefixAttributeController>()
				.SetRoutePrefix("RoutePrefixDE")
				.ForAction(x => x.Index())
				.AddTranslation("Willkommen")
				.ForAction(x => x.Book(0, 0))
				.AddTranslation("Buch/{chapter}/{page}");

			localization.ForCulture("de")
				.ForController<ControllerLevelAttributeController>()
				.AddTranslation("ControllerEbeneAttribut/{action}")
				.ForAction(x => x.Book(0, 0))
				.AddTranslation("ControllerEbeneAttribut/Buch/{chapter}/{page}");
		}
	}
}