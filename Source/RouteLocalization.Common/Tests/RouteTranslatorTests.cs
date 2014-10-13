#if ASPNETWEBAPI
namespace RouteLocalization.Http.Tests
#else
namespace RouteLocalization.Mvc.Tests
#endif
{
#if ASPNETWEBAPI
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Setup;
	using RouteLocalization.Http.Tests.Core;
	using TRoute = System.Web.Http.Routing.HttpRoute;
	using TRouteValueDictionary = System.Web.Http.Routing.HttpRouteValueDictionary;
#else
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Mvc;
	using System.Web.Mvc.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;
	using RouteLocalization.Mvc.Tests.Core;
	using TRoute = System.Web.Routing.Route;
	using TRouteValueDictionary = System.Web.Routing.RouteValueDictionary;
#endif

	[TestClass]
	public class RouteTranslatorTests
	{
		protected Configuration Configuration { get; set; }

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Multiple Routes with different Url found for given Controller 'Home' and Action 'Index'. Narrow down your selection.")]
		public void
			AddTranslation_ConfigurationAddTranslationToSimiliarUrlsIsTrueWithDifferentUrl_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AddTranslationToSimiliarUrls = true;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome2", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()
				.AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);
		}

		[TestMethod]
		public void AddTranslation_ConfigurationAddTranslationToSimiliarUrlsIsTrue_TranslateMultipleRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AddTranslationToSimiliarUrls = true;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()
				.AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") != null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de").Url() == "Willkommen");

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") != null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de").Url() == "Willkommen");
		}

		[TestMethod]
		public void
			AddTranslation_ConfigurationAttributeRouteProcessingIsAddAsNeutralRouteAndReplaceByFirstTranslation_RemovesNeutralRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralRouteAndReplaceByFirstTranslation;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()
				.AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") != null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de").Url() == "Willkommen");

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		[TestMethod]
		public void AddTranslation_ConfigurationAddCultureAsRoutePrefixIsTrue_AddsPrefixToTranslatedRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AddCultureAsRoutePrefix = true;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "de/Willkommen");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateCultureIsFalse_IgnoresNotAcceptedCulture()
		{
			// Arrange
			Configuration.ValidateCulture = false;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);
		}

#if !ASPNETWEBAPI
		[TestMethod]
		public void AddTranslation_ConfigurationValidateRouteAreaIsFalse_IgnoresMissingRouteAreaAttribute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateRouteArea = false;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController<MissingAttributeController>()
				.ForAction(x => x.Index())
				.SetAreaPrefix("de")
				.AddTranslation("Start", "de");
		}
#endif

		[TestMethod]
		public void AddTranslation_ConfigurationValidateRoutePrefixIsFalse_IgnoresMissingRoutePrefixAttribute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateRoutePrefix = false;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController<MissingAttributeController>()
				.ForAction(x => x.Index())
				.SetRoutePrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateUrlIsFalse_IgnoresDifferentNumberOfUrlPlaceholders()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateUrl = false;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Book/{chapter}/{page}",
					controller => controller.Book(0, 0));

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Buch/{chapter}/", "de", "Home", "Book", string.Empty, null);
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateUrlIsFalse_IgnoresDifferentUrlPlaceholders()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateUrl = false;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Book/{chapter}/{page}",
					controller => controller.Book(0, 0));

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Buch/{page}/{chapter}", "de", "Home", "Book", string.Empty, null);
		}

		[TestMethod]
		public void AddTranslation_MixedCaseAcceptedCulture_IsCaseInsensitive()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de-AT");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Serwas", "DE-at", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de-at").Url() == "Serwas");
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("DE-AT").Url() == "Serwas");
		}

		[TestMethod]
		public void AddTranslation_DefaultConfig_PassesCultureAsDefault()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(!Configuration.LocalizationCollectionRoutes.Single().Route.Defaults.ContainsKey("culture"));
			Assert.IsTrue((string)localizationCollectionRoute.GetLocalizedRoute("de").Defaults["culture"] == "de");
		}

		[TestMethod]
		public void AddTranslation_DefaultConfig_AddTranslatedControllerLevelRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerLevelRoute<HomeController>("Controller/{action}", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Kontrolleur/{action}", "de", "Home", null, string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "Kontrolleur/{action}");
		}

		[TestMethod]
		public void AddTranslation_DefaultConfig_AddTranslatedRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "Willkommen");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException), "Route already has translation for culture 'de'.")]
		public void AddTranslationTwice_DefaultConfig_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);
			(new Localization(Configuration)).AddTranslation("Willkommen", "de", "Home", "Index", string.Empty, null);
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Translation Route 'Buch/{chapter}/' contains different number of { } placeholders than original Route 'Book/{chapter}/{page}'." +
				"Set Configuration.ValidateURL to false, if you want to skip validation.")]
		public void AddTranslation_DifferentNumberOfUrlPlaceholders_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Book/{chapter}/{page}",
					controller => controller.Book(0, 0));

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Buch/{chapter}/", "de", "Home", "Book", string.Empty, null);
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Translation Route 'Buch/{page}/{chapter}' contains different { } placeholders than original Route 'Book/{chapter}/{page}'." +
				"Set Configuration.ValidateURL to false, if you want to skip validation.")]
		public void AddTranslation_DifferentUrlPlaceholders_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Book/{chapter}/{page}",
					controller => controller.Book(0, 0));

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Buch/{page}/{chapter}", "de", "Home", "Book", string.Empty, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddTranslation_EmptyCulture_ThrowsArgumentNullException()
		{
			// Arrange
			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController("Home", string.Empty)
				.ForAction("Index")
				.AddTranslation("Start", string.Empty);
		}

#if !ASPNETWEBAPI
		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"AreaPrefix is set but Controller 'RouteLocalization.Mvc.Tests.Core.MissingAttributeController' does not contain any RouteArea attributes." +
		"Set Configuration.ValidateRouteArea to false, if you want to skip validation.")]
		public void AddTranslation_MissingRouteAreaAttribute_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController<MissingAttributeController>()
				.ForAction(x => x.Index())
				.SetAreaPrefix("de")
				.AddTranslation("Start", "de");
		}
#endif

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException), "RoutePrefix is set but Controller 'RouteLocalization." +
#if ASPNETWEBAPI
			"Http" +
#else
			"Mvc" +
#endif
			".Tests.Core.MissingAttributeController' does not contain any RoutePrefix attributes." +
			"Set Configuration.ValidateRoutePrefix to false, if you want to skip validation.")]
		public void AddTranslation_MissingRoutePrefixAttribute_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<MissingAttributeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController<MissingAttributeController>()
				.ForAction(x => x.Index())
				.SetRoutePrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"No Route found for given Controller 'Home' and Action 'Index'.")]
		public void AddTranslation_MissingRoute_ThrowsInvalidOperationException()
		{
			// Act
			(new Localization(Configuration)).AddTranslation("Start", "de", "Home", "Index", string.Empty, null);
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException), "AcceptedCultures does not contain culture 'de'.")]
		public void AddTranslation_NotAcceptedCulture_ThrowsInvalidOperationException()
		{
			// Arrange
			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).AddTranslation("Start", "de", "Home", "Index", string.Empty, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddTranslation_NullController_ThrowsArgumentNullException()
		{
			// Act
			(new Localization(Configuration)).ForController(null, string.Empty).ForAction("Index").AddTranslation("Start", "de");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddTranslation_NullCulture_ThrowsArgumentNullException()
		{
			// Arrange
			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).ForController("Home", string.Empty)
				.ForAction("Index")
				.AddTranslation("Start", null);
		}

		[TestMethod]
		public void AddTranslation_PrefixesExistsNoPrefixGiven_UsesPrefixValueFromAttributes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;
			Configuration.AddCultureAsRoutePrefix = true;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<PrefixController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()
				.AddTranslation("Willkommen", "de", "Prefix", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);

#if ASPNETWEBAPI
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("en").Url() == "en/RoutePrefix/Welcome");
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "de/RoutePrefix/Willkommen");
#else
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("en").Url() == "en/RouteArea/RoutePrefix/Welcome");
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "de/RouteArea/RoutePrefix/Willkommen");
#endif
		}

		[TestMethod]
		public void AddTranslation_PrefixesExists_AddsPrefixesInCorrectOrderToTranslatedRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;
			Configuration.AddCultureAsRoutePrefix = true;

			LocalizationCollectionRoute localizationCollectionRoute =
				CreateCollectionRouteForControllerAndAction<PrefixController>("Welcome", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()

#if !ASPNETWEBAPI
				.SetAreaPrefix("Area")
#endif

				.SetRoutePrefix("Route")
				.AddTranslation("Willkommen", "de", "Prefix", "Index", string.Empty, null);

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 1);

#if ASPNETWEBAPI
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("en").Url() == "en/RoutePrefix/Welcome");
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "de/Route/Willkommen");
#else
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("en").Url() == "en/RouteArea/RoutePrefix/Welcome");
			Assert.IsTrue(localizationCollectionRoute.GetLocalizedRoute("de").Url() == "de/Area/Route/Willkommen");
#endif
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Multiple Routes found for given Controller 'Home' and Action 'Index'. " +
				"Narrow down your selection or use AddTranslationToSimiliarUrls if you want to translate similiar Routes at once.")]
		public void AddTranslation_TwoRoutesMatches_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome1", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome2", controller => controller.Index());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes()
				.AddTranslation("Willkommen1", "de", "Home", "Index", string.Empty, null);
		}

		[TestMethod]
		public void ForAndSetFunctions_DefaultRouteTranslator_SavesValues()
		{
			// Arrange
			string controllerName = "Home";
			string controllerNamespace = "Namespace";
			string actionName = "Index";

#if !ASPNETWEBAPI
			string areaPrefix = "AreaPrefix";
#endif

			string routePrefix = "RoutePrefix";
			string culture = "de";

			// Act
			RouteTranslator routeTranslator =
				new RouteTranslator().ForController(controllerName, controllerNamespace)
					.ForAction(actionName)

#if !ASPNETWEBAPI
					.SetAreaPrefix(areaPrefix)
#endif

					.SetRoutePrefix(routePrefix)
					.ForCulture(culture);

			// Assert
			Assert.IsTrue(routeTranslator.Controller == controllerName);
			Assert.IsTrue(routeTranslator.ControllerNamespace == controllerNamespace);
			Assert.IsTrue(routeTranslator.Action == actionName);

#if !ASPNETWEBAPI
			Assert.IsTrue(routeTranslator.AreaPrefix == areaPrefix);
#endif

			Assert.IsTrue(routeTranslator.RoutePrefix == routePrefix);
			Assert.IsTrue(routeTranslator.Culture == culture);
		}

		[TestMethod]
		public void StronglyTypedForAndSetFunctions_DefaultRouteTranslator_SavesValues()
		{
			// Arrange
			string controllerName = "MissingAttribute";
			string controllerNamespace = typeof(MissingAttributeController).Namespace;
			string actionName = "Index";

#if !ASPNETWEBAPI
			string areaPrefix = "AreaPrefix";
#endif

			string routePrefix = "RoutePrefix";
			string culture = "de";

			// Act
			RouteTranslator<MissingAttributeController> routeTranslator =
				new RouteTranslator().ForController<MissingAttributeController>()
					.ForAction(x => x.Index())

#if !ASPNETWEBAPI
					.SetAreaPrefix(areaPrefix)
#endif

					.SetRoutePrefix(routePrefix)
					.ForCulture(culture);

			// Assert
			Assert.IsTrue(routeTranslator.Controller == controllerName);
			Assert.IsTrue(routeTranslator.ControllerNamespace == controllerNamespace);
			Assert.IsTrue(routeTranslator.Action == actionName);

#if !ASPNETWEBAPI
			Assert.IsTrue(routeTranslator.AreaPrefix == areaPrefix);
#endif

			Assert.IsTrue(routeTranslator.RoutePrefix == routePrefix);
			Assert.IsTrue(routeTranslator.Culture == culture);
		}

		[TestInitialize]
		public void TestInitialize()
		{
			Configuration = new Configuration
			{
				DefaultCulture = "en",
				AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralRoute,
				AddCultureAsRoutePrefix = false,
				AcceptedCultures = new HashSet<string>() { "en" },
				ValidateUrl = true,

#if !ASPNETWEBAPI
				ValidateRouteArea = true,
#endif

				ValidateRoutePrefix = true,
				ValidateCulture = true,
				AddTranslationToSimiliarUrls = false
			};
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsDefaultCultureRoute_PassesCultureAsDefault()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.All(entry => !entry.Route.Defaults.ContainsKey("culture")));
			Assert.IsTrue((string)localizationCollectionRoute1.GetLocalizedRoute("en").Defaults["culture"] == "en");
			Assert.IsTrue((string)localizationCollectionRoute2.GetLocalizedRoute("en").Defaults["culture"] == "en");
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsDefaultCultureRoute_ContainsDefaultCultureRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") != null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en").Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") == null);

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") != null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en").Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsNeutralAndDefaultCultureRoute_ContainsNeutralAndDefaultCultureRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") != null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en").Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") == null);

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") != null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en").Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsNeutralAndDefaultCultureRoute_PassesCultureAsDefault()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.All(entry => !entry.Route.Defaults.ContainsKey("culture")));
			Assert.IsTrue((string)localizationCollectionRoute1.NeutralRoute.Defaults["culture"] == string.Empty);
			Assert.IsTrue((string)localizationCollectionRoute2.NeutralRoute.Defaults["culture"] == string.Empty);
			Assert.IsTrue((string)localizationCollectionRoute1.GetLocalizedRoute("en").Defaults["culture"] == "en");
			Assert.IsTrue((string)localizationCollectionRoute2.GetLocalizedRoute("en").Defaults["culture"] == "en");
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsNeutralRouteAndReplaceByFirstTranslation_ContainsNeutralRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralRouteAndReplaceByFirstTranslation;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") == null);

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AddAsNeutralRoute_ContainsNeutralRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralRoute;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") == null);

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute != null);
			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute.Url() == "Welcome");
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		[TestMethod]
		public void TranslateInitialAttributeRoutes_AttributeRouteProcessingIsNone_DoesNotContainAnyRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");
			Configuration.AttributeRouteProcessing = AttributeRouteProcessing.None;

			LocalizationCollectionRoute localizationCollectionRoute1 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index());
			LocalizationCollectionRoute localizationCollectionRoute2 =
				CreateCollectionRouteForControllerAndAction<HomeController>("Welcome", controller => controller.Index2());

			Configuration.LocalizationCollectionRoutes = new List<RouteEntry>
			{
				new RouteEntry(string.Empty, localizationCollectionRoute1),
				new RouteEntry(string.Empty, localizationCollectionRoute2)
			};

			// Act
			(new Localization(Configuration)).TranslateInitialAttributeRoutes();

			// Assert
			Assert.IsTrue(Configuration.LocalizationCollectionRoutes.Count == 2);

			Assert.IsTrue(localizationCollectionRoute1.NeutralRoute == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute1.GetLocalizedRoute("de") == null);

			Assert.IsTrue(localizationCollectionRoute2.NeutralRoute == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("en") == null);
			Assert.IsTrue(localizationCollectionRoute2.GetLocalizedRoute("de") == null);
		}

		protected LocalizationCollectionRoute CreateCollectionRouteForControllerLevelRoute<T>(string url,
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
							RouteDataTokenKeys.Controller,
							new HttpControllerDescriptor(new HttpConfiguration(), typeof(T).Name.Substring(0, typeof(T).Name.Length - 10),
								typeof(T))
						},
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
						}
#endif
					}, null));
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