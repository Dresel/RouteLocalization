namespace RouteLocalization.AspNetCore.Test.Selection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Selection;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class UntranslatedRoutesRouteSelectorBuilderTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		public static DefaultLocalizer GetLocalizer()
		{
			return new DefaultLocalizer(UntranslatedRoutesRouteSelectorBuilderTest.CultureKey);
		}

		[TestMethod]
		public void BuildForTranslatedControllerAndTranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller1()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizer()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.SingleOrDefault(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsNull(routeSelection);
		}

		[TestMethod]
		public void BuildForTranslatedControllerAndUntranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller1()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsTrue(routeSelection.ActionModels.Count == 3);
		}

		[TestMethod]
		public void BuildForUntranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller2()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller2");

			Assert.IsTrue(routeSelection.ActionModels.Count == 2);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerAndTranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller1()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizer()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsTrue(routeSelection.ActionModels.Count == 0);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerAndUntranslatedActionsRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller1()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsTrue(routeSelection.ActionModels.Count == 3);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new UntranslatedRoutesRouteSelectorBuilder()
			{
				Culture = TranslatedRoutesRouteSelectorBuilderTest.Culture,
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			ControllerModel translatedController = applicationModel.Controller3()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection =
				routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller3");

			Assert.IsTrue(routeSelection.ActionModels.Count == 0);
		}
	}
}