namespace RouteLocalization.AspNetCore.Test.Processor
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Processor;
	using RouteLocalization.AspNetCore.Selection;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class CopyTemplateRouteProcessorBuilderTest
	{
		public const string CultureKey = "cultureKey";

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectTranslation()
		{
			CopyTemplateRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = applicationModel.Controller1().Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller2") == 1);
			Assert.IsTrue(translatedController.Selectors.Count == 2);
			Assert.IsTrue(translatedController.Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller1");
			Assert.IsTrue(translatedController.Selectors.Last().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller11");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 2);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template == "Action1");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Last().AttributeRouteModel.Template == "Action11");
			Assert.IsTrue(translatedController.Actions.Skip(1).All(action => action.Selectors.Count == 0));
		}

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectTranslation2()
		{
			CopyTemplateRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = applicationModel.Controller1().Actions.Skip(1).Take(2).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 2);
			Assert.IsTrue(translatedController.Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller1");
			Assert.IsTrue(translatedController.Selectors.Last().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller11");
			Assert.IsTrue(translatedController.Actions.Skip(1).First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.Skip(2).First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.Skip(1).First().Selectors.First().AttributeRouteModel.Template ==
				"Action2");
			Assert.IsTrue(
				translatedController.Actions.Skip(1).First().Selectors.Last().AttributeRouteModel.Template == "Action2");
		}

		[TestMethod]
		public void BuildWithControllerSelectionBuildsCorrectTranslation()
		{
			CopyTemplateRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = new List<ActionModel>()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 2);
			Assert.IsTrue(translatedController.Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller1");
			Assert.IsTrue(translatedController.Selectors.Last().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller11");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 0);
		}

		[TestMethod]
		public void BuildWithControllerSelectionMixBuildsCorrectTranslation()
		{
			CopyTemplateRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller3(),
					ActionModels = new List<ActionModel>()
				},
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = applicationModel.Controller1().Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller2") == 1);
			Assert.IsTrue(translatedController.Selectors.Count == 2);
			Assert.IsTrue(translatedController.Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller1");
			Assert.IsTrue(translatedController.Selectors.Last().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller11");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 2);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template == "Action1");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Last().AttributeRouteModel.Template == "Action11");
			Assert.IsTrue(translatedController.Actions.Skip(1).All(action => action.Selectors.Count == 0));
		}

		[TestMethod]
		public void BuildWithMultipleControllerActionsSelectionBuildsCorrectTranslation()
		{
			CopyTemplateRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = applicationModel.Controller1().Actions.Take(1).ToList()
				},
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller2(),
					ActionModels = applicationModel.Controller2().Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 2);
			Assert.IsTrue(translatedController.Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller1");
			Assert.IsTrue(translatedController.Selectors.Last().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Controller11");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 2);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template == "Action1");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Last().AttributeRouteModel.Template == "Action11");
			Assert.IsTrue(translatedController.Actions.Skip(1).All(action => action.Selectors.Count == 0));

			translatedController = applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller2");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller2") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 0);
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/Action1");
			Assert.IsTrue(translatedController.Actions.Last().Selectors.Count == 0);
		}

		public CopyTemplateRouteProcessor CreateRouteProcessor()
		{
			return new CopyTemplateRouteProcessor(new RouteTranslationConfiguration()
			{
				Localizer = new DefaultLocalizer("cultureKey")
			});
		}
	}
}