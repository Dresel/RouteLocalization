namespace RouteLocalizationMVC.Tests
{
	using System.Web.Mvc;
	using System.Web.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalizationMVC.Extensions;

	[TestClass]
	public class RouteCollectionExtensionsTests
	{
		[TestMethod]
		public void GetFirstUntranslatedRoute_NoRouteExists_ReturnsNull()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection();

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_OneRouteExistsFirstHasTranslation_ReturnsNull()
		{
			// Arrange
			TranslationRoute route1 =
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			route1.TranslatedRoutes["de"] =
				new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			RouteCollection routeCollection = new RouteCollection() { route1 };

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_ThreeRoutesExistsFirstHasTranslationSecondIsTranslation_ReturnsThird()
		{
			// Arrange
			TranslationRoute route1 =
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			route1.TranslatedRoutes["de"] =
				new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			route1.TranslatedRoutes["de"].TranslationRouteRoot = route1;

			RouteCollection routeCollection = new RouteCollection
			{
				route1,
				route1.TranslatedRoutes["de"],
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler())
			};

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.AreSame(routeCollection[2], route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoRoutesExistsFirstHasTranslation_ReturnsSecond()
		{
			// Arrange
			TranslationRoute route1 =
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			route1.TranslatedRoutes["de"] =
				new Route("Start", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute();

			RouteCollection routeCollection = new RouteCollection
			{
				route1,
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler())
			};

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.AreSame(routeCollection[1], route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoRoutesExists_ReturnsFirst()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection
			{
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()),
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler())
			};

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.AreSame(routeCollection[0], route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoTranslationRouteExists_ReturnsFirst()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection
			{
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute(),
				new Route("Home", new RouteValueDictionary() { { "controller", "Home" }, { "action", "Index" } },
					new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler()).ToTranslationRoute()
			};

			// Act
			Route route = routeCollection.GetFirstUntranslatedRoute("de", "Home", "Index");

			// Assert
			Assert.AreSame(routeCollection[0], route);
		}
	}
}