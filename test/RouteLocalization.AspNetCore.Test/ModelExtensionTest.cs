namespace RouteLocalization.AspNetCore.Test
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class ModelExtensionTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		public static DefaultLocalizer GetLocalizedRouteMarker()
		{
			return new DefaultLocalizer(ModelExtensionTest.CultureKey);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetOriginalModelForOriginalActionThrowsException()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			ActionModel originalAction = controller.Actions.First().GetOriginalModel(GetLocalizedRouteMarker());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetOriginalModelForOriginalControllerThrowsException()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			ControllerModel originalController = controller.GetOriginalModel(GetLocalizedRouteMarker());
		}

		[TestMethod]
		public void GetOriginalModelForLocalizedActionReturnsOriginalModel()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			ActionModel originalAction = controller
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture)
				.Actions.First()
				.GetOriginalModel(GetLocalizedRouteMarker());

			Assert.AreSame(controller.Actions.First(), originalAction);
		}

		[TestMethod]
		public void GetOriginalModelForLocalizedControllerReturnsOriginalModel()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			ControllerModel originalController = controller
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture)
				.GetOriginalModel(GetLocalizedRouteMarker());

			Assert.AreSame(controller, originalController);
		}

		[TestMethod]
		public void GetLocalizedModelForOriginalActionReturnsLocalizedAction()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			ActionModel translatedAction2 = action.GetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.AreSame(translatedAction, translatedAction2);
		}

		[TestMethod]
		public void GetLocalizedModelForOriginalControllerReturnsLocalizedController()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 =
				controller.GetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.AreSame(translatedController, translatedController2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetLocalizedModelForLocalizedActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			ActionModel translatedAction2 =
				translatedAction.GetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetLocalizedModelForLocalizedControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 =
				translatedController.GetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
		}

		[TestMethod]
		public void HasAttributeRoutesForAttributeRoutedActionReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsTrue(controller.Actions.First().HasAttributeRoutes());
		}

		[TestMethod]
		public void HasAttributeRoutesForAttributeRoutedControllerReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsTrue(controller.HasAttributeRoutes());
		}

		[TestMethod]
		public void HasAttributeRoutesForNonAttributeRoutedActionReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller3();

			Assert.IsFalse(controller.Actions.First().HasAttributeRoutes());
		}

		[TestMethod]
		public void HasAttributeRoutesForNonAttributeRoutedControllerReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller2();

			Assert.IsFalse(controller.HasAttributeRoutes());
		}

		[TestMethod]
		public void IsCompletelyTranslatedForCompletelyLocalizedActionReturnsTrue()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			foreach (SelectorModel selectorModel in action.Selectors)
			{
				translatedAction.Selectors.Add(new SelectorModel(selectorModel));
			}

			bool isCompletelyLocalized = translatedAction.IsCompletelyTranslated(GetLocalizedRouteMarker());

			Assert.IsTrue(isCompletelyLocalized);
		}

		[TestMethod]
		public void IsCompletelyTranslatedForCompletelyLocalizedControllerReturnsTrue()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			foreach (SelectorModel selectorModel in controller.Selectors)
			{
				translatedController.Selectors.Add(new SelectorModel(selectorModel));
			}

			bool isCompletelyLocalized = translatedController.IsCompletelyTranslated(GetLocalizedRouteMarker());

			Assert.IsTrue(isCompletelyLocalized);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsCompletelyTranslatedForNonAttributeRoutedActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller3();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = translatedAction.IsCompletelyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsCompletelyTranslatedForNonAttributeRoutedControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller2();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = translatedController.IsCompletelyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		public void IsCompletelyTranslatedForNotLocalizedActionReturnsFalse()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = translatedAction.IsCompletelyTranslated(GetLocalizedRouteMarker());

			Assert.IsFalse(isCompletelyLocalized);
		}

		[TestMethod]
		public void IsCompletelyTranslatedForNotLocalizedControllerReturnsFalse()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = translatedController.IsCompletelyTranslated(GetLocalizedRouteMarker());

			Assert.IsFalse(isCompletelyLocalized);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsCompletelyTranslatedForOriginalActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = action.IsCompletelyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsCompletelyTranslatedForOriginalControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isCompletelyLocalized = controller.IsCompletelyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		public void IsOriginalModelForOriginalActionReturnsTrue()
		{
			ActionModel action = TestApplicationModel.Instance.Controller1().Actions.First();

			Assert.IsTrue(action.IsOriginalModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsOriginalModelForOriginalControllerReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsTrue(controller.IsOriginalModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsOriginalModelForLocalizedActionReturnsFalse()
		{
			ActionModel action = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture)
				.Actions.First();

			Assert.IsFalse(action.IsOriginalModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsOriginalModelForLocalizedControllerReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsFalse(controller.IsOriginalModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsPartiallyTranslatedForNonAttributeRoutedActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller3();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = translatedAction.IsPartiallyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsPartiallyTranslatedForNonAttributeRoutedControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller2();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = translatedController.IsPartiallyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		public void IsPartiallyTranslatedForNotLocalizedActionReturnsFalse()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = translatedAction.IsPartiallyTranslated(GetLocalizedRouteMarker());

			Assert.IsFalse(isPartiallyLocalized);
		}

		[TestMethod]
		public void IsPartiallyTranslatedForNotLocalizedControllerReturnsFalse()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = translatedController.IsPartiallyTranslated(GetLocalizedRouteMarker());

			Assert.IsFalse(isPartiallyLocalized);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsPartiallyTranslatedForOriginalActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = action.IsPartiallyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void IsPartiallyTranslatedForOriginalControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			bool isPartiallyLocalized = controller.IsPartiallyTranslated(GetLocalizedRouteMarker());
		}

		[TestMethod]
		public void IsPartiallyTranslatedForPartiallyLocalizedActionReturnsTrue()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			foreach (SelectorModel selectorModel in action.Selectors.Take(1))
			{
				translatedAction.Selectors.Add(new SelectorModel(selectorModel));
			}

			bool isPartiallyLocalized = translatedAction.IsPartiallyTranslated(GetLocalizedRouteMarker());

			Assert.IsTrue(isPartiallyLocalized);
		}

		[TestMethod]
		public void IsPartiallyTranslatedForPartiallyLocalizedControllerReturnsTrue()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			foreach (SelectorModel selectorModel in controller.Selectors.Take(1))
			{
				translatedController.Selectors.Add(new SelectorModel(selectorModel));
			}

			bool isPartiallyLocalized = translatedController.IsPartiallyTranslated(GetLocalizedRouteMarker());

			Assert.IsTrue(isPartiallyLocalized);
		}

		[TestMethod]
		public void IsLocalizedModelForForLocalizedActionReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsTrue(controller.Actions.First()
				.IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForForLocalizedActionWithDifferentCultureReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture2);

			Assert.IsFalse(
				controller.Actions.First().IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForForLocalizedControllerReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsTrue(controller.IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForForLocalizedControllerWithDifferentCultureReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture2);

			Assert.IsFalse(controller.IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForForUntranslatedActionReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsFalse(
				controller.Actions.First().IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForForUntranslatedControllerReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsFalse(controller.IsLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture));
		}

		[TestMethod]
		public void IsLocalizedModelForLocalizedActionReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsTrue(controller.Actions.First().IsLocalizedModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsLocalizedModelForLocalizedControllerReturnsTrue()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1()
				.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsTrue(controller.IsLocalizedModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsLocalizedModelForUntranslatedActionReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsFalse(controller.Actions.First().IsLocalizedModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void IsLocalizedModelForUntranslatedControllerReturnsFalse()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsFalse(controller.IsLocalizedModel(GetLocalizedRouteMarker()));
		}

		[TestMethod]
		public void MatchesActionArgumentsWithArgumentListMatchesExact()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			ICollection<Type> actionArguments = new[] { typeof(int) };

			Assert.IsFalse(controller.Actions.Single(action => action.ActionName == "Action1")
				.MatchesActionArguments(actionArguments));
			Assert.IsFalse(controller.Actions.Where(action => action.ActionName == "Action2")
				.All(action => !action.MatchesActionArguments(actionArguments)));
			Assert.IsTrue(controller.Actions.Single(action => action.ActionName == "Action3")
				.MatchesActionArguments(actionArguments));
			Assert.IsFalse(controller.Actions.Single(action => action.ActionName == "Action4")
				.MatchesActionArguments(actionArguments));
		}

		[TestMethod]
		public void MatchesActionArgumentsWithArgumentListMatchesExact2()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			IList<ParameterModel> actionArguments = controller.Actions.Single(action => action.ActionName == "Action3")
				.Parameters;

			Assert.IsFalse(controller.Actions.Single(action => action.ActionName == "Action1")
				.MatchesActionArguments(actionArguments));
			Assert.IsFalse(controller.Actions.Where(action => action.ActionName == "Action2")
				.All(action => !action.MatchesActionArguments(actionArguments)));
			Assert.IsTrue(controller.Actions.Single(action => action.ActionName == "Action3")
				.MatchesActionArguments(actionArguments));
			Assert.IsFalse(controller.Actions.Single(action => action.ActionName == "Action4")
				.MatchesActionArguments(actionArguments));
		}

		[TestMethod]
		public void MatchesActionArgumentsWithNullArgumentListMatchesAny()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsTrue(controller.Actions.All(action => action.MatchesActionArguments((ICollection<Type>)null)));
		}

		[TestMethod]
		public void MatchesActionArgumentsWithNullArgumentListMatchesAny2()
		{
			ControllerModel controller = TestApplicationModel.Instance.Controller1();

			Assert.IsTrue(controller.Actions.All(action => action.MatchesActionArguments((ICollection<ParameterModel>)null)));
		}

		[TestMethod]
		public void TryGetLocalizedModelForOriginalActionIfMissingReturnsNull()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();

			ActionModel translatedAction = controller.Actions.First()
				.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsNull(translatedAction);
		}

		[TestMethod]
		public void TryGetLocalizedModelForOriginalActionReturnsLocalizedAction()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			ActionModel translatedAction2 =
				action.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.AreSame(translatedAction, translatedAction2);
		}

		[TestMethod]
		public void TryGetLocalizedModelForOriginalControllerIfMissingReturnsNull()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();

			ControllerModel translatedController =
				controller.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.IsNull(translatedController);
		}

		[TestMethod]
		public void TryGetLocalizedModelForOriginalControllerReturnsLocalizedController()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 =
				controller.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			Assert.AreSame(translatedController, translatedController2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryGetLocalizedModelForLocalizedActionThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ActionModel action = controller.Actions.First();

			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
			ActionModel translatedAction = translatedController.Actions.First();

			applicationModel.Controllers.Add(translatedController);

			ActionModel translatedAction2 =
				translatedAction.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryGetLocalizedModelForLocalizedControllerThrowsException()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel controller = applicationModel.Controller1();
			ControllerModel translatedController =
				controller.CreateLocalizedControllerModel(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 =
				translatedController.TryGetLocalizedModelFor(GetLocalizedRouteMarker(), ModelExtensionTest.Culture);
		}
	}
}