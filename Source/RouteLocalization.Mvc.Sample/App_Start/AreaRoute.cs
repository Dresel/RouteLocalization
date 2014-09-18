namespace RouteLocalization.Mvc.Sample
{
	using RouteLocalization.Mvc.Sample.Areas.Area.Controllers;

	public static class AreaRoute
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