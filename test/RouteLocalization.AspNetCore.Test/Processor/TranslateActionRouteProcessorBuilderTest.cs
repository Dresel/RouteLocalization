namespace RouteLocalization.AspNetCore.Test.Processor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Processor;
	using RouteLocalization.AspNetCore.Selection;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class TranslateActionRouteProcessorBuilderTest
	{
		public const string CultureKey = "cultureKey";

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectTranslation()
		{
			TranslateActionRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

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
			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 0);
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 2);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template == "template");
			Assert.IsTrue(translatedController.Actions.First().Selectors.Last().AttributeRouteModel.Template == "template");
		}

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectTranslation2()
		{
			TranslateActionRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller2(),
					ActionModels = applicationModel.Controller2().Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			ControllerModel translatedController =
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller2");

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller2") == 2);
			Assert.IsTrue(translatedController.Selectors.Count == 0);
			Assert.IsTrue(translatedController.Actions.First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.First().Selectors.First().AttributeRouteModel.Template ==
				$"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/template");
		}

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectTranslation3()
		{
			TranslateActionRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

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
			Assert.IsTrue(translatedController.Selectors.Count == 0);
			Assert.IsTrue(translatedController.Actions.Skip(1).First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.Skip(2).First().Selectors.Count == 1);
			Assert.IsTrue(translatedController.Actions.Skip(1).First().Selectors.First().AttributeRouteModel.Template ==
				"template");
			Assert.IsTrue(translatedController.Actions.Skip(2).First().Selectors.Last().AttributeRouteModel.Template ==
				"template");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BuildWithControllerWithoutActionsSelectionThrowsException()
		{
			TranslateActionRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1()
				}
			};

			routeProcessor.Process(routeSelections);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BuildWithMultipleSelectionBuildsThrowsException()
		{
			TranslateActionRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

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
		}

		public TranslateActionRouteProcessor CreateRouteProcessor()
		{
			return new TranslateActionRouteProcessor(new RouteTranslationConfiguration()
			{
				Localizer = new DefaultLocalizer("cultureKey")
			});
		}
	}
}