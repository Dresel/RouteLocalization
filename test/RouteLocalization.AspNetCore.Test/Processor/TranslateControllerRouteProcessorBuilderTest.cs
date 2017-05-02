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
	public class TranslateControllerRouteProcessorBuilderTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		[TestMethod]
		public void BuildWithControllerSelectionBuildsCorrectTranslation()
		{
			TranslateControllerRouteProcessor routeProcessor = CreateRouteProcessor();
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
			routeProcessor.Process(routeSelections);

			Assert.IsTrue(applicationModel.Controllers.Count(controller => controller.ControllerName == "Controller1") == 2);
			Assert.IsTrue(
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1")
					.Selectors.First()
					.AttributeRouteModel.Template == $"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/template");
			Assert.IsTrue(
				applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1")
					.Selectors.Last()
					.AttributeRouteModel.Template == $"[{TranslateControllerRouteProcessorBuilderTest.CultureKey}]/template");
			Assert.IsTrue(applicationModel.Controllers.Last(controller => controller.ControllerName == "Controller1")
				.Actions.All(action => action.Selectors.Count == 0));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BuildWithControllerWithActionsSelectionThrowsException()
		{
			TranslateControllerRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1(),
					ActionModels = applicationModel.Controller1().Actions
				}
			};

			routeProcessor.Process(routeSelections);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BuildWithMultipleSelectionThrowsException()
		{
			TranslateControllerRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Culture = TranslateControllerRouteProcessorBuilderTest.Culture;
			routeProcessor.Template = "template";

			ApplicationModel applicationModel = TestApplicationModel.Instance;

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller1()
				},
				new RouteSelection()
				{
					ControllerModel = applicationModel.Controller2()
				}
			};

			routeProcessor.Process(routeSelections);
		}

		public TranslateControllerRouteProcessor CreateRouteProcessor()
		{
			return new TranslateControllerRouteProcessor(new RouteTranslationConfiguration()
			{
				Localizer = new DefaultLocalizer("cultureKey")
			});
		}
	}
}