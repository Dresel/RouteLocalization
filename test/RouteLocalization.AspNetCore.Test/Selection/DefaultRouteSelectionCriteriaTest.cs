namespace RouteLocalization.AspNetCore.Test.Selection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Selection;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class DefaultRouteSelectionCriteriaTest
	{
		[TestMethod]
		public void DefaultRouteSelectionCriteriaWithControllerAndActionCriteriaReturnsCorrectSelection()
		{
			IRouteSelector routeSelector = new BasicRouteCriteriaRouteSelector(new DefaultLocalizer())
			{
				Controller = "Controller1",
				ControllerNamespace = typeof(Controller1).Namespace,
				Action = "Action1"
			};

			ICollection<RouteSelection> routeSelections = routeSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.Single().ActionModels.Single().ActionName == "Action1");
		}

		[TestMethod]
		public void DefaultRouteSelectionCriteriaWithControllerAndActionCriteriaReturnsCorrectSelection2()
		{
			IRouteSelector routeSelector = new BasicRouteCriteriaRouteSelector(new DefaultLocalizer())
			{
				Controller = "Controller1",
				ControllerNamespace = typeof(Controller1).Namespace,
				Action = "Action2"
			};

			ICollection<RouteSelection> routeSelections = routeSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Single().ActionModels.First().ActionName == "Action2");
		}

		[TestMethod]
		public void DefaultRouteSelectionCriteriaWithControllerAndActionCriteriaReturnsCorrectSelection3()
		{
			IRouteSelector routeSelector = new BasicRouteCriteriaRouteSelector(new DefaultLocalizer())
			{
				Controller = "Controller1",
				ControllerNamespace = typeof(Controller1).Namespace,
				Action = "Action2",
				ActionArguments = new List<Type>()
				{
				}
			};

			ICollection<RouteSelection> routeSelections = routeSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.Single().ActionModels.First().ActionName == "Action2");
		}

		[TestMethod]
		public void DefaultRouteSelectionCriteriaWithControllerCriteriaReturnsControllerSelection()
		{
			IRouteSelector routeSelector = new BasicRouteCriteriaRouteSelector(new DefaultLocalizer())
			{
				Controller = "Controller1",
				ControllerNamespace = typeof(Controller1).Namespace
			};

			ICollection<RouteSelection> routeSelections = routeSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller1");
			Assert.IsNull(routeSelections.Single().ActionModels);
		}

		[TestMethod]
		public void DefaultRouteSelectionCriteriaWithControllerCriteriaReturnsEmptySelection()
		{
			IRouteSelector routeSelector = new BasicRouteCriteriaRouteSelector(new DefaultLocalizer())
			{
				Controller = "MissingController",
				ControllerNamespace = typeof(Controller1).Namespace
			};

			ICollection<RouteSelection> routeSelections = routeSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 0);
		}
	}
}