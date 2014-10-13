#if ASPNETWEBAPI
namespace RouteLocalization.Http.Tests
#else
namespace RouteLocalization.Mvc.Tests
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

#if ASPNETWEBAPI
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
	using System.Web.Mvc;
	using System.Web.Mvc.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Tests.Core;
	using TIRoute = System.Web.Routing.Route;
	using TRoute = System.Web.Routing.Route;
	using TRouteValueDictionary = System.Web.Routing.RouteValueDictionary;
#endif

	[TestClass]
	public class RouteEntryCollectionExtensionTests
	{
		[TestMethod]
		public void GetRoutes_NoRouteExistsForNamespace_ReturnsEmptyList()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty,
					CreateCollectionRouteForControllerAndAction<Core.DifferentNamespace.HomeController>("Home", x => x.Index()))
			};

			// Act
			ICollection<LocalizationCollectionRoute> routes = routeEntries.GetRoutes("Home", "Index", "Namespace3", null);

			// Assert
			Assert.IsTrue(routes.Count == 0);
		}

		[TestMethod]
		public void GetRoutes_NoRouteExists_ReturnsEmptyList()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>();

			// Act
			ICollection<LocalizationCollectionRoute> routes = routeEntries.GetRoutes("Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(routes.Count == 0);
		}

		[TestMethod]
		public void GetRoutes_TwoRoutesWithNamespacesExists_ReturnsCorrectRoute()
		{
			// Arrange
			List<RouteEntry> routeEntries = new List<RouteEntry>()
			{
				new RouteEntry(string.Empty, CreateCollectionRouteForControllerAndAction<HomeController>("Home", x => x.Index())),
				new RouteEntry(string.Empty,
					CreateCollectionRouteForControllerAndAction<Core.DifferentNamespace.HomeController>("Home", x => x.Index())),
			};

			// Act
			ICollection<LocalizationCollectionRoute> routes = routeEntries.GetRoutes("Home", "Index",
				typeof(Core.DifferentNamespace.HomeController).Namespace, null);

			// Assert
			Assert.IsTrue(routes.Count == 1);
			Assert.AreSame(routeEntries[1].Route, routes.First());
		}

		[TestMethod]
		public void GetRoutes_WithActionParameterSpecified_ReturnsCorrectRoute()
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
			ICollection<LocalizationCollectionRoute> routes =
				routeEntries.GetRoutes("MissingAttribute", "Index", null, new Type[] { });

			// Assert
			Assert.IsTrue(routes.Count == 1);
			Assert.AreSame(route1, routes.First());
		}

		[TestMethod]
		public void GetRoutes_WithActionParameterSpecified_ReturnsCorrectRoute2()
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
			ICollection<LocalizationCollectionRoute> routes = routeEntries.GetRoutes("MissingAttribute", "Index", null, new[] { typeof(int) });

			// Assert
			Assert.IsTrue(routes.Count == 1);
			Assert.AreSame(route2, routes.First());
		}

		[TestMethod]
		public void GetNamedRoute_NoRouteExists_ReturnsNull()
		{
			// Arrange
			ICollection<RouteEntry> routeEntries = new List<RouteEntry>();

			// Act
			TIRoute route = routeEntries.GetNamedRoute("Route1");

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
			TIRoute route = routeEntries.GetNamedRoute("Route1");

			// Assert
			Assert.AreSame(route1, route);
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