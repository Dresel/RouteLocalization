namespace RouteLocalizationMVC.Tests
{
	using System.Globalization;
	using System.Threading;
	using System.Web.Mvc;
	using System.Web.Routing;
	using System.Web.Routing.Fakes;
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalizationMVC.Extensions;

	[TestClass]
	public class TranslationRouteTests
	{
		[TestMethod]
		public void GetVirtualPath_CultureOverridenByRouteValueDictionaryMultipleTranslations_ReturnsTranslationRoutePath()
		{
			using (ShimsContext.Create())
			{
				// Arrange		
				ShimRoute shimRouteRoot =
					new ShimRoute(
						new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteRoot.GetVirtualPathRequestContextRouteValueDictionary = (requestContext, values) => null;

				TranslationRoute translationRouteRoot = (TranslationRoute)shimRouteRoot.Instance;
				translationRouteRoot.Culture = "en";

				ShimRoute shimRouteDE =
					new ShimRoute(
						new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteDE.GetVirtualPathRequestContextRouteValueDictionary = (requestContext, values) => null;

				TranslationRoute translationRouteDE = (TranslationRoute)shimRouteDE.Instance;
				translationRouteDE.Culture = "de";

				translationRouteRoot.TranslatedRoutes.Add(translationRouteDE.Culture, translationRouteDE);

				ShimRoute shimRouteRootWelcome =
					new ShimRoute(
						new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Welcome" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteRootWelcome.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteRootWelcome, "Welcome");

				TranslationRoute translationRouteRootWelcome = (TranslationRoute)shimRouteRootWelcome.Instance;
				translationRouteRootWelcome.Culture = "en";

				ShimRoute shimRouteWelcomeDE =
					new ShimRoute(
						new Route("Willkommen", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Welcome" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteWelcomeDE.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteWelcomeDE, "Willkommen");

				TranslationRoute translationRouteWelcomeDE = (TranslationRoute)shimRouteWelcomeDE.Instance;
				translationRouteWelcomeDE.Culture = "de";

				translationRouteRootWelcome.TranslatedRoutes.Add(translationRouteWelcomeDE.Culture, translationRouteWelcomeDE);

				CultureInfo culture = new CultureInfo("en");

				Thread.CurrentThread.CurrentUICulture = culture;
				Thread.CurrentThread.CurrentCulture = culture;

				RouteValueDictionary routeValueDictionary = new RouteValueDictionary() { { "Culture", "de" } };

				VirtualPathData virtualPathDataFirst = translationRouteRoot.GetVirtualPath(null, routeValueDictionary);
				VirtualPathData virtualPathDataSecond = translationRouteRootWelcome.GetVirtualPath(null, routeValueDictionary);

				// Assert
				Assert.IsNull(virtualPathDataFirst);
				Assert.IsTrue(virtualPathDataSecond.VirtualPath == "Willkommen");
			}
		}

		[TestMethod]
		public void GetVirtualPath_CultureOverridenByRouteValueDictionary_ReturnsTranslationRoutePath()
		{
			using (ShimsContext.Create())
			{
				// Arrange							
				ShimRoute shimRouteRoot =
					new ShimRoute(
						new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteRoot.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteRoot, "Home");

				TranslationRoute translationRouteRoot = (TranslationRoute)shimRouteRoot.Instance;
				translationRouteRoot.Culture = "en";

				ShimRoute shimRouteDE =
					new ShimRoute(
						new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteDE.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteDE, "Start");

				TranslationRoute translationRouteDE = (TranslationRoute)shimRouteDE.Instance;
				translationRouteDE.Culture = "de";

				ShimRoute shimRouteFR =
					new ShimRoute(
						new Route("Debut", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteFR.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteDE, "Debut");

				TranslationRoute translationRouteFR = (TranslationRoute)shimRouteFR.Instance;
				translationRouteFR.Culture = "fr";

				translationRouteRoot.TranslatedRoutes.Add(translationRouteDE.Culture, translationRouteDE);
				translationRouteRoot.TranslatedRoutes.Add(translationRouteFR.Culture, translationRouteFR);

				CultureInfo culture = new CultureInfo("de");

				Thread.CurrentThread.CurrentUICulture = culture;
				Thread.CurrentThread.CurrentCulture = culture;

				VirtualPathData virtualPathData = translationRouteRoot.GetVirtualPath(null,
					new RouteValueDictionary() { { "Culture", "fr" } });

				// Assert
				Assert.IsTrue(virtualPathData.VirtualPath == "Debut");
			}
		}

		[TestMethod]
		public void GetVirtualPath_MissingTranslationForThreadCulture_ReturnsTranslationRootRoutePath()
		{
			using (ShimsContext.Create())
			{
				// Arrange							
				ShimRoute shimRouteRoot =
					new ShimRoute(
						new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteRoot.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteRoot, "Home");

				TranslationRoute translationRouteRoot = (TranslationRoute)shimRouteRoot.Instance;
				translationRouteRoot.Culture = "en";

				TranslationRoute translationRoute =
					new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
						new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

				translationRoute.Culture = "de";

				translationRouteRoot.TranslatedRoutes.Add(translationRoute.Culture, translationRoute);

				CultureInfo culture = new CultureInfo("fr");

				Thread.CurrentThread.CurrentUICulture = culture;
				Thread.CurrentThread.CurrentCulture = culture;

				VirtualPathData virtualPathData = translationRouteRoot.GetVirtualPath(null, new RouteValueDictionary());

				// Assert
				Assert.IsTrue(virtualPathData.VirtualPath == "Home");
			}
		}

		[TestMethod]
		public void GetVirtualPath_TranslationExistsForThreadCulture_ReturnsTranslationRoutePath()
		{
			using (ShimsContext.Create())
			{
				// Arrange							
				ShimRoute shimRouteRoot =
					new ShimRoute(
						new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRouteRoot.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRouteRoot, "Home");

				TranslationRoute translationRouteRoot = (TranslationRoute)shimRouteRoot.Instance;
				translationRouteRoot.Culture = "en";

				ShimRoute shimRoute =
					new ShimRoute(
						new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
							new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute());

				shimRoute.GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(shimRoute, "Start");

				TranslationRoute translationRoute = (TranslationRoute)shimRoute.Instance;
				translationRoute.Culture = "de";

				translationRouteRoot.TranslatedRoutes.Add(translationRoute.Culture, translationRoute);

				CultureInfo culture = new CultureInfo("de");

				Thread.CurrentThread.CurrentUICulture = culture;
				Thread.CurrentThread.CurrentCulture = culture;

				VirtualPathData virtualPathData = translationRouteRoot.GetVirtualPath(null, new RouteValueDictionary());

				// Assert
				Assert.IsTrue(virtualPathData.VirtualPath == "Start");
			}
		}
	}
}