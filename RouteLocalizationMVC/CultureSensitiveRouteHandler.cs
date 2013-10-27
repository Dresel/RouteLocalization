namespace RouteLocalizationMVC
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Web;
	using System.Web.Routing;

	public class CultureSensitiveRouteHandler : IRouteHandler
	{
		public static EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public CultureSensitiveRouteHandler(IRouteHandler routeHandler)
		{
			RouteHandler = routeHandler;
		}

		protected IRouteHandler RouteHandler { get; set; }

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			TranslationRoute route = requestContext.RouteData.Route as TranslationRoute;

			// Route doesn't contain culture information so return
			if (route == null || string.IsNullOrEmpty(route.Culture))
			{
				return RouteHandler.GetHttpHandler(requestContext);
			}

			// Route contains culture information
			string cultureName = route.Culture;

			// Set culture
			CultureInfo cultureInfo = new CultureInfo(cultureName);

			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureName });

			return RouteHandler.GetHttpHandler(requestContext);
		}
	}
}