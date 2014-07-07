namespace RouteLocalization.Mvc.Tests
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using System.Web.Routing;
	using System.Web.Routing.Fakes;
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Routing.Fakes;

	[TestClass]
	public class LocalizationCollectionRouteTests
	{
		[TestMethod]
		public void GetVirtualPath_NoTranslationExistsForThreadCulture_ReturnsNeutralLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new Route("Home",
						new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new RouteValueDictionary(),
						new RouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), string.Empty));

				(new ShimRoute(localizationRouteNeutral)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteNeutral, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "en"));

				(new ShimRoute(localizationRouteEnglish)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteEnglish, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Willkommen", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimRoute(localizationRouteGerman)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteGerman, "Willkommen");

				// Act
				VirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null, new RouteValueDictionary());

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteNeutral.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverridenCultureByRouteDictionaryThatDoesNotExist_ReturnsNeutralLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new Route("Home",
						new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new RouteValueDictionary(),
						new RouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), string.Empty));

				(new ShimRoute(localizationRouteNeutral)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteNeutral, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "en"));

				(new ShimRoute(localizationRouteEnglish)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteEnglish, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Willkommen", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimRoute(localizationRouteGerman)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteGerman, "Willkommen");

				// Act
				VirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new RouteValueDictionary() { { "Culture", "es" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteNeutral.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverridenCultureByRouteDictionary_ReturnsCorrectTranslatedLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new Route("Home",
						new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new RouteValueDictionary(),
						new RouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), string.Empty));

				(new ShimRoute(localizationRouteNeutral)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteNeutral, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "en"));

				(new ShimRoute(localizationRouteEnglish)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteEnglish, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Willkommen", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimRoute(localizationRouteGerman)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteGerman, "Willkommen");

				// Act
				VirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new RouteValueDictionary() { { "Culture", "de" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteGerman.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_TranslationExistsForThreadCulture_ReturnsTranslatedLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new Route("Home",
						new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new RouteValueDictionary(),
						new RouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), string.Empty));

				(new ShimRoute(localizationRouteNeutral)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteNeutral, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Welcome", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "en"));

				(new ShimRoute(localizationRouteEnglish)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteEnglish, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(
						new LocalizationRoute(
							new Route("Willkommen", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
								new RouteValueDictionary(), new RouteValueDictionary(), null), "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimRoute(localizationRouteGerman)).GetVirtualPathRequestContextRouteValueDictionary =
					(requestContext, values) => new VirtualPathData(localizationRouteGerman, "Willkommen");

				// Act
				VirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null, new RouteValueDictionary());

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteEnglish.Instance);
			}
		}
	}
}