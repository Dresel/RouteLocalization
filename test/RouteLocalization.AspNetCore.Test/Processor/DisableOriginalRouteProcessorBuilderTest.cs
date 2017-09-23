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
	public class DisableOriginalRouteProcessorBuilderTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		public static DefaultLocalizer GetLocalizedRouteMarker()
		{
			return new DefaultLocalizer(ModelExtensionTest.CultureKey);
		}

		[TestMethod]
		public void BuildWithActionsSelectionBuildsCorrectProcessor()
		{
			DisableOriginalRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Cultures = new[]
				{ DisableOriginalRouteProcessorBuilderTest.Culture, DisableOriginalRouteProcessorBuilderTest.Culture2 };

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller2();

			ControllerModel translatedController =
				originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (ActionModel translatedAction in translatedController.Actions.Take(1))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
					.Take(1))
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (ActionModel translatedAction in translatedController.Actions.Take(1))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = originalController,
					ActionModels = originalController.Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			Assert.IsFalse(originalController.Selectors.First().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Actions.Last().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
		}

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectProcessor()
		{
			DisableOriginalRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Cultures = new[]
				{ DisableOriginalRouteProcessorBuilderTest.Culture, DisableOriginalRouteProcessorBuilderTest.Culture2 };

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController =
				originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions.Take(1))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
					.Take(1))
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions.Take(1))
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = originalController,
					ActionModels = originalController.Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			Assert.IsFalse(originalController.Selectors.First().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Actions.First().Selectors.Last().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
		}

		[TestMethod]
		public void BuildWithControllerActionsSelectionBuildsCorrectProcessor2()
		{
			DisableOriginalRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Cultures = new[]
				{ DisableOriginalRouteProcessorBuilderTest.Culture, DisableOriginalRouteProcessorBuilderTest.Culture2 };

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController =
				originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = originalController,
					ActionModels = originalController.Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			Assert.IsTrue(originalController.Selectors.First().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Selectors.Last().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.Last().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
		}

		[TestMethod]
		public void BuildWithControllerSelectionBuildsCorrectProcessor()
		{
			DisableOriginalRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Cultures = new[]
				{ DisableOriginalRouteProcessorBuilderTest.Culture, DisableOriginalRouteProcessorBuilderTest.Culture2 };

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller3();

			ControllerModel translatedController =
				originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = originalController,
					ActionModels = new List<ActionModel>()
				}
			};

			routeProcessor.Process(routeSelections);

			Assert.IsTrue(originalController.Selectors.First().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Selectors.Last().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Actions.First().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
		}

		[TestMethod]
		public void BuildWithControllerSelectionMixBuildsCorrectProcessor()
		{
			DisableOriginalRouteProcessor routeProcessor = CreateRouteProcessor();
			routeProcessor.Cultures = new[]
				{ DisableOriginalRouteProcessorBuilderTest.Culture, DisableOriginalRouteProcessorBuilderTest.Culture2 };

			ApplicationModel applicationModel = TestApplicationModel.Instance;
			ControllerModel originalController = applicationModel.Controller1();

			ControllerModel translatedController =
				originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			translatedController = originalController.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			foreach (ActionModel translatedAction in translatedController.Actions)
			{
				foreach (SelectorModel selectorModel in translatedAction.GetOriginalModel(GetLocalizedRouteMarker()).Selectors)
				{
					translatedAction.Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			ControllerModel originalController3 = applicationModel.Controller3();

			ControllerModel translatedController3 =
				originalController3.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
					DisableOriginalRouteProcessorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController3.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController3.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController3);

			translatedController3 = originalController3.CreateLocalizedControllerModel(GetLocalizedRouteMarker(),
				DisableOriginalRouteProcessorBuilderTest.Culture2);

			foreach (SelectorModel selectorModel in translatedController3.GetOriginalModel(GetLocalizedRouteMarker()).Selectors
				.Take(1))
			{
				translatedController3.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController3);

			List<RouteSelection> routeSelections = new List<RouteSelection>()
			{
				new RouteSelection()
				{
					ControllerModel = originalController3,
					ActionModels = new List<ActionModel>()
				},
				new RouteSelection()
				{
					ControllerModel = originalController,
					ActionModels = originalController.Actions.Take(1).ToList()
				}
			};

			routeProcessor.Process(routeSelections);

			Assert.IsTrue(originalController.Selectors.First().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsFalse(originalController.Selectors.Last().ActionConstraints.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.First().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
			Assert.IsTrue(originalController.Actions.First().Selectors.Last().ActionConstraints
				.OfType<NeverAcceptActionContraint>().Any());
		}

		public DisableOriginalRouteProcessor CreateRouteProcessor()
		{
			return new DisableOriginalRouteProcessor(new RouteTranslationConfiguration()
			{
				Localizer = new DefaultLocalizer("cultureKey")
			});
		}
	}
}