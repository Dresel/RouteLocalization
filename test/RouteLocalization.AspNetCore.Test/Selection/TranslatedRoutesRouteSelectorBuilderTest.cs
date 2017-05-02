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
	public class TranslatedRoutesRouteSelectorBuilderTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		public static DefaultLocalizer GetLocalizer()
		{
			return new DefaultLocalizer(ModelExtensionTest.CultureKey);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerAndUntranslatedActionsRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controllerModel = applicationModel.Controller1();

			ControllerModel translatedController = controllerModel
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			translatedController = controllerModel
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 0);
		}

		[TestMethod]
		public void BuildForUntranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller2();

			ControllerModel translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 0);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller3();

			ControllerModel translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 0);
		}

		[TestMethod]
		public void BuildForUntranslatedControllerAndTranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (ActionModel translatedAction in translatedController.Actions.Take(2))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizer()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			foreach (ActionModel translatedAction in translatedController.Actions.Skip(1).Take(2))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizer()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			RouteSelection routeSelection = routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsTrue(routeSelection.ActionModels.Count == 1);
		}

		[TestMethod]
		public void BuildForTranslatedControllerAndUntranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			ICollection<RouteSelection> routeSelections = routeSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 0);
		}

		[TestMethod]
		public void BuildForTranslatedControllerAndTranslatedActionRoutesReturnsCorrectRouteSelector()
		{
			IRouteSelector routeSelector = new TranslatedRoutesRouteSelector()
			{
				Cultures = new[] { UntranslatedRoutesRouteSelectorBuilderTest.Culture, UntranslatedRoutesRouteSelectorBuilderTest.Culture2 },
				Localizer = GetLocalizer()
			};

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (ActionModel translatedAction in translatedController.Actions.Take(2))
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

			translatedController = originalController
				.CreateLocalizedControllerModel(GetLocalizer(),
					UntranslatedRoutesRouteSelectorBuilderTest.Culture2);

			foreach (ActionModel translatedAction in translatedController.Actions.Skip(1).Take(2))
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

			RouteSelection routeSelection = routeSelections.Single(selection => selection.ControllerModel.ControllerName == "Controller1");

			Assert.IsTrue(routeSelection.ActionModels.Count == 1);
		}
	}
}