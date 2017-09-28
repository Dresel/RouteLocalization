namespace RouteLocalization.AspNetCore.Test.Selection
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.AspNetCore.Selection;
	using RouteLocalization.AspNetCore.Test.TestData;

	[TestClass]
	public class FilterRouteSelectorTest
	{
		public const string Culture = "en";

		public const string Culture2 = "de";

		public const string CultureKey = "cultureKey";

		public static DefaultLocalizer GetLocalizer()
		{
			return new DefaultLocalizer(ModelExtensionTest.CultureKey);
		}

		[TestMethod]
		public void SelectWithFilteredActionReturnsCorrectSelection()
		{
			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller1(),
				ActionModels = TestApplicationModel.Instance.Controller1().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller2(),
				ActionModels = TestApplicationModel.Instance.Controller2().Actions
			});

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller1",
				Action = "Action1"
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ControllerModel.ControllerName == "Controller1");
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 4);
			Assert.IsFalse(routeSelections.First().ActionModels.Any(x => x.ActionName == "Action1"));
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredControllerReturnsCorrectSelection()
		{
			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller1(),
				ActionModels = TestApplicationModel.Instance.Controller1().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller2(),
				ActionModels = TestApplicationModel.Instance.Controller2().Actions
			});

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller1"
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Single().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoTranslatedOptionReturnsCorrectSelection1()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController2 = applicationModel.Controller2()
				.CreateLocalizedControllerModel(GetLocalizer(), FilterRouteSelectorTest.Culture);

			for (int i = 0; i < translatedController2.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController2.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController2.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController2);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoTranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Single().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoTranslatedOptionReturnsCorrectSelection2()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 = applicationModel.Controller2()
				.CreateLocalizedControllerModel(GetLocalizer(), FilterRouteSelectorTest.Culture);

			for (int i = 0; i < translatedController2.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController2.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController2.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController2);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoTranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoTranslatedOptionReturnsCorrectSelection3()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			for (int i = 0; i < translatedController.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 = applicationModel.Controller2()
				.CreateLocalizedControllerModel(GetLocalizer(), FilterRouteSelectorTest.Culture);

			for (int i = 0; i < translatedController2.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController2.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController2.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController2);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoTranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.First().ActionModels.Single().Parameters.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoTranslatedOptionReturnsCorrectSelection4()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			for (int i = 0; i < translatedController.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			ControllerModel translatedController2 = applicationModel.Controller2()
				.CreateLocalizedControllerModel(GetLocalizer(), FilterRouteSelectorTest.Culture);

			for (int i = 0; i < translatedController2.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController2.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController2.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController2);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoTranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.First().ActionModels.Single().Parameters.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoUntranslatedOptionReturnsCorrectSelection1()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoUntranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.First().ActionModels.Single().Parameters.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoUntranslatedOptionReturnsCorrectSelection2()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			applicationModel.Controllers.Add(translatedController);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoUntranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.First().ActionModels.Single().Parameters.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoUntranslatedOptionReturnsCorrectSelection3()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Selectors)
			{
				translatedController.Selectors.Add(selectorModel);
			}

			for (int i = 0; i < translatedController.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoUntranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 1);
			Assert.IsTrue(routeSelections.Single().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Single().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Single().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Single().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredNoUntranslatedOptionReturnsCorrectSelection4()
		{
			ApplicationModel applicationModel = TestApplicationModel.Instance;

			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller4(),
				ActionModels = applicationModel.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = applicationModel.Controller2(),
				ActionModels = applicationModel.Controller2().Actions
			});

			ControllerModel translatedController = applicationModel.Controller4()
				.CreateLocalizedControllerModel(GetLocalizer(), TranslatedRoutesRouteSelectorBuilderTest.Culture);

			for (int i = 0; i < translatedController.Actions.Count; i++)
			{
				foreach (SelectorModel selectorModel in translatedController.GetOriginalModel(GetLocalizer()).Actions[i].Selectors)
				{
					translatedController.Actions[i].Selectors.Add(selectorModel);
				}
			}

			applicationModel.Controllers.Add(translatedController);

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) },
				Localizer = GetLocalizer(),
				Cultures = new[] { FilterRouteSelectorTest.Culture },
				FilterControllerOrActionWhenNoUntranslatedRouteLeft = true
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(applicationModel);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredSimilarActionWithActionArgumentsReturnsCorrectSelection()
		{
			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller4(),
				ActionModels = TestApplicationModel.Instance.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller2(),
				ActionModels = TestApplicationModel.Instance.Controller2().Actions
			});

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1",
				ActionArguments = new[] { typeof(int) }
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 1);
			Assert.IsTrue(routeSelections.First().ActionModels.Single().Parameters.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}

		[TestMethod]
		public void SelectWithFilteredSimilarActionWithoutActionArgumentsReturnsCorrectSelection()
		{
			StaticRouteSelector routeSelector = new StaticRouteSelector();

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller4(),
				ActionModels = TestApplicationModel.Instance.Controller4().Actions
			});

			routeSelector.RouteSelection.Add(new RouteSelection()
			{
				ControllerModel = TestApplicationModel.Instance.Controller2(),
				ActionModels = TestApplicationModel.Instance.Controller2().Actions
			});

			FilterRouteSelector filterRouteSelector = new FilterRouteSelector(routeSelector)
			{
				Controller = "Controller4",
				Action = "Action1"
			};

			ICollection<RouteSelection> routeSelections = filterRouteSelector.Select(TestApplicationModel.Instance);

			Assert.IsTrue(routeSelections.Count == 2);
			Assert.IsTrue(routeSelections.First().ActionModels.Count == 0);
			Assert.IsTrue(routeSelections.Last().ControllerModel.ControllerName == "Controller2");
			Assert.IsTrue(routeSelections.Last().ActionModels.Count == 2);
			Assert.IsTrue(routeSelections.Last().ActionModels.First().ActionName == "Action1");
			Assert.IsTrue(routeSelections.Last().ActionModels.Last().ActionName == "Action2");
		}
	}
}