#if ASPNETWEBAPI
namespace RouteLocalization.Http.Tests
#else
namespace RouteLocalization.Mvc.Tests
#endif
{
#if ASPNETWEBAPI
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Tests.Core;
	using TIRoute = System.Web.Http.Routing.IHttpRoute;
	using TRoute = System.Web.Http.Routing.HttpRoute;
	using TRouteValueDictionary = System.Web.Http.Routing.HttpRouteValueDictionary;
#else
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Mvc;
	using System.Web.Mvc.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;
	using RouteLocalization.Mvc.Tests.Core;
	using TIRoute = System.Web.Routing.Route;
	using TRoute = System.Web.Routing.Route;
	using TRouteValueDictionary = System.Web.Routing.RouteValueDictionary;
#endif

	[TestClass]
	public class RouteEntryCollectionExtensionTests
	{
		[TestMethod]
		public void GetFirstUntranslatedRoute_NoRouteExistsForNamespace_ReturnsNull()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty,
					CreateCollectionRouteForControllerAndAction<Core.DifferentNamespace.HomeController>("Home", x => x.Index()))
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index", "Namespace3", null);

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_NoRouteExists_ReturnsNull()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>();

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_OneRouteExistsFirstHasTranslation_ReturnsNull()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			route1.AddTranslation("Start", "de");

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>() { new RouteEntry(string.Empty, route1) };

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoRoutesExistsFirstHasTranslation_ReturnsSecond()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			route1.AddTranslation("Start", "de");

			LocalizationCollectionRoute route2 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, route1),
				new RouteEntry(string.Empty, route2)
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.AreSame(route2, route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoRoutesExists_ReturnsFirst()
		{
			// Arrange
			List<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.AreSame(routeEntries[0].Route, route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_TwoRoutesWithNamespacesExists_ReturnsCorrectRoute()
		{
			// Arrange
			List<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty,
					CreateCollectionRouteForControllerAndAction<Core.DifferentNamespace.HomeController>("Home", x => x.Index())),
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "Home", "Index",
				typeof(Core.DifferentNamespace.HomeController).Namespace, null);

			// Assert
			Assert.AreSame(routeEntries[1].Route, route);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_WithActionParameterSpecified_ReturnsCorrectRoute()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Home",
				x => x.Index());

			LocalizationCollectionRoute route2 = CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Home",
				x => x.Index(0));

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, route1),
				new RouteEntry(string.Empty, route2)
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "MissingAttribute", "Index", null, new Type[] { });

			// Assert
			Assert.AreSame(route, route1);
		}

		[TestMethod]
		public void GetFirstUntranslatedRoute_WithActionParameterSpecified_ReturnsCorrectRoute2()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Home",
				x => x.Index());

			LocalizationCollectionRoute route2 = CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Home",
				x => x.Index(0));

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, route1),
				new RouteEntry(string.Empty, route2)
			};

			// Act
			TIRoute route = routeEntries.GetFirstUntranslatedRoute("de", "MissingAttribute", "Index", null, new[] { typeof(int) });

			// Assert
			Assert.AreSame(route, route2);
		}

		[TestMethod]
		public void GetNamedRoute_NoRouteExists_ReturnsNull()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>();

			// Act
			TIRoute route = routeEntries.GetNamedRoute("de", "Route1");

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetNamedRoute_RouteExistsAndHasNoTranslation_ReturnsRoute()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>() { new RouteEntry("Route1", route1) };

			// Act
			TIRoute route = routeEntries.GetNamedRoute("de", "Route1");

			// Assert
			Assert.AreSame(route1, route);
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Named route already has translation for culture 'de'.")]
		public void GetNamedRoute_RouteExistsAndHasTranslation_ThrowsException()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			route1.AddTranslation("Start", "de");

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>() { new RouteEntry("Route1", route1) };

			// Act
			TIRoute route = routeEntries.GetNamedRoute("de", "Route1");

			// Assert
			Assert.IsNull(route);
		}

		[TestMethod]
		public void GetSimiliarUntranslatedRoutes_TwoRoutesExistsFirstHasTranslation_ReturnsSecond()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			route1.AddTranslation("Start", "de");

			LocalizationCollectionRoute route2 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, route1),
				new RouteEntry(string.Empty, route2)
			};

			// Act
			ICollection<LocalizationCollectionRoute> localizationCollectionRoutes =
				routeEntries.GetSimiliarUntranslatedRoutes("de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.AreSame(route2, localizationCollectionRoutes.Single());
		}

		[TestMethod]
		public void GetSimiliarUntranslatedRoutes_TwoRoutesExists_ReturnsBoth()
		{
			// Arrange
			LocalizationCollectionRoute route1 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			LocalizationCollectionRoute route2 = CreateCollectionRouteForControllerAndAction<HomeController>("Home",
				x => x.Index());

			ICollection<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, route1),
				new RouteEntry(string.Empty, route2)
			};

			// Act
			ICollection<LocalizationCollectionRoute> routes = routeEntries.GetSimiliarUntranslatedRoutes("de", "Home", "Index",
				string.Empty, null);

			// Assert
			Assert.IsTrue(routes.Count() == 2);
		}

		[TestMethod]
		public void GetSimiliarUntranslatedRoutes_TwoRoutesWithDifferentNamespacesExists_ReturnsCorrectRoute()
		{
			// Arrange
			List<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty,
					CreateCollectionRouteForControllerAndAction<Core.DifferentNamespace.HomeController>("Home", x => x.Index()))
			};

			// Act
			ICollection<LocalizationCollectionRoute> localizationCollectionRoutes =
				routeEntries.GetSimiliarUntranslatedRoutes("de", "Home", "Index",
					typeof(Core.DifferentNamespace.HomeController).Namespace, null);

			// Assert
			Assert.AreSame(routeEntries[1].Route, localizationCollectionRoutes.Single());
		}

		[TestMethod]
		public void GetSimiliarUntranslatedRoutes_TwoRoutesWithSameNamespaceExists_ReturnsBoth()
		{
			// Arrange
			List<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index()))
			};

			// Act
			ICollection<LocalizationCollectionRoute> localizationCollectionRoutes =
				routeEntries.GetSimiliarUntranslatedRoutes("de", "Home", "Index", typeof(HomeController).Namespace, null);

			// Assert
			Assert.IsTrue(localizationCollectionRoutes.Count == 2);
		}

		protected LocalizationCollectionRoute CreateCollectionRouteForControllerAndAction<T>(string url,
			Expression<Func<T, object>> expression)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			if (methodCall == null)
			{
				throw new ArgumentException("Expression must be a MethodCallExpression", "expression");
			}

			MethodInfo info = methodCall.Method;

			return
				new LocalizationCollectionRoute(new TRoute(url, new TRouteValueDictionary(), new TRouteValueDictionary(),
					new TRouteValueDictionary()
					{
#if ASPNETWEBAPI
						{
							RouteDataTokenKeys.Actions,
							new[]
							{
								new ReflectedHttpActionDescriptor(
									new HttpControllerDescriptor(new HttpConfiguration(), typeof(T).Name.Substring(0, typeof(T).Name.Length - 10),
										typeof(T)), info)
							}
						}
#else
						{
							RouteDataTokenKeys.Actions,
							new[] { new ReflectedActionDescriptor(info, info.Name, new ReflectedControllerDescriptor(typeof(T))) }
						},
						{ RouteDataTokenKeys.TargetIsAction, true }
#endif
					}, null));
		}
	}
}