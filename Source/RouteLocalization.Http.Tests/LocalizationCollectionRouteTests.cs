namespace RouteLocalization.Http.Tests
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using System.Web.Http.Routing;
	using System.Web.Http.Routing.Fakes;
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Routing.Fakes;

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
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary());

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteNeutral.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverriddenCultureByRouteDictionaryThatDoesNotExist_ReturnsNeutralLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary() { { "Culture", "es" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteNeutral.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverriddenCultureByRouteDictionary_ReturnsCorrectTranslatedLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary() { { "Culture", "de" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteGerman.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverriddenRegionCultureByRouteDictionaryThatDoesNotExist_ReturnsCorrectTranslatedLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary() { { "Culture", "de-AT" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteGerman.Instance);
			}
		}

		[TestMethod]
		public void GetVirtualPath_OverriddenRegionCultureByRouteDictionary_ReturnsCorrectTranslatedLocalizationRoute()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

				LocalizationCollectionRoute localizationCollectionRoute =
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				ShimLocalizationRoute localizationRouteGermanAustria =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de-AT"));

				routes["de-AT"] = localizationRouteGermanAustria;

				(new ShimHttpRoute(localizationRouteGermanAustria)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGermanAustria.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary() { { "Culture", "de-AT" } });

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteGermanAustria.Instance);
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
					new LocalizationCollectionRoute(new HttpRoute("Home",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null));

				PrivateObject privateObject = new PrivateObject(localizationCollectionRoute);

				IDictionary<string, LocalizationRoute> routes =
					(IDictionary<string, LocalizationRoute>)privateObject.GetProperty("LocalizedRoutesContainer");

				ShimLocalizationRoute localizationRouteNeutral =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, string.Empty));

				(new ShimHttpRoute(localizationRouteNeutral)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteNeutral.Instance, "Welcome");

				routes[string.Empty] = localizationRouteNeutral;

				ShimLocalizationRoute localizationRouteEnglish =
					new ShimLocalizationRoute(new LocalizationRoute("Welcome",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "en"));

				(new ShimHttpRoute(localizationRouteEnglish)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteEnglish.Instance, "Welcome");

				routes["en"] = localizationRouteEnglish;

				ShimLocalizationRoute localizationRouteGerman =
					new ShimLocalizationRoute(new LocalizationRoute("Willkommen",
						new HttpRouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } }, new HttpRouteValueDictionary(),
						new HttpRouteValueDictionary(), null, "de"));

				routes["de"] = localizationRouteGerman;

				(new ShimHttpRoute(localizationRouteGerman)).GetVirtualPathHttpRequestMessageIDictionaryOfStringObject =
					(requestContext, values) => new HttpVirtualPathData(localizationRouteGerman.Instance, "Willkommen");

				// Act
				IHttpVirtualPathData virtualPathData = localizationCollectionRoute.GetVirtualPath(null,
					new HttpRouteValueDictionary());

				// Assert
				Assert.AreEqual(virtualPathData.Route, localizationRouteEnglish.Instance);
			}
		}
	}
}