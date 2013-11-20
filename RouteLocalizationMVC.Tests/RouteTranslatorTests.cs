namespace RouteLocalizationMVC.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using System.Web.Routing;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using RouteLocalizationMVC.Extensions;
	using RouteLocalizationMVC.Setup;
	using RouteLocalizationMVC.Tests.Core;

	[TestClass]
	public class RouteTranslatorTests
	{
		protected Configuration Configuration { get; set; }

		[TestMethod]
		public void AddTranslation_ConfigurationAddCultureAsRoutePrefixIsTrue_AddsPrefixToTranslatedRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.AddCultureAsRoutePrefix = true;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Welcome", "Welcome", new { controller = "Home", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Willkommen", "de", "Home", "Index");

			// Assert
			Assert.IsTrue(routeCollection.Count == 2);

			Assert.IsTrue(((TranslationRoute)routeCollection[0]).Url == "en/Welcome");
			Assert.IsTrue(((TranslationRoute)routeCollection[1]).Url == "de/Willkommen");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateCultureIsFalse_IgnoresNotAcceptedCulture()
		{
			// Arrange
			Configuration.ValidateCulture = false;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Start", "de", "Home", "Index");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateRouteAreaIsFalse_IgnoresMissingRouteAreaAttribute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateRouteArea = false;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "MissingAttribute", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).ForController<MissingAttributeController>().ForAction(x => x.Index()).SetAreaPrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateRoutePrefixIsFalse_IgnoresMissingRoutePrefixAttribute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateRoutePrefix = false;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "MissingAttribute", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).ForController<MissingAttributeController>().ForAction(x => x.Index()).SetRoutePrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateURLIsFalse_IgnoresDifferentNumberOfURLPlaceholders()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateURL = false;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/", "de", "Home", "Book");
		}

		[TestMethod]
		public void AddTranslation_ConfigurationValidateURLIsFalse_IgnoresDifferentURLPlaceholders()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.ValidateURL = false;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{page}/{chapter}", "de", "Home", "Book");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Translation Route 'Buch/{chapter}/' contains different number of { } placeholders than original Route 'Book/{chapter}/{page}'." +
				"Set Configuration.ValidateURL to false, if you want to skip validation.")]
		public void AddTranslation_DifferentNumberOfURLPlaceholders_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/", "de", "Home", "Book");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Translation Route 'Buch/{page}/{chapter}' contains different { } placeholders than original Route 'Book/{chapter}/{page}'." +
				"Set Configuration.ValidateURL to false, if you want to skip validation.")]
		public void AddTranslation_DifferentURLPlaceholders_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{page}/{chapter}", "de", "Home", "Book");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Controller 'RouteLocalizationMVC.Tests.Core.MissingAttributeController' does not contain any RouteArea attributes.")
		]
		public void AddTranslation_MissingRouteAreaAttribute_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "MissingAttribute", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).ForController<MissingAttributeController>().ForAction(x => x.Index()).SetAreaPrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"Controller 'RouteLocalizationMVC.Tests.Core.MissingAttributeController' does not contain any RoutePrefix attributes."
			)]
		public void AddTranslation_MissingRoutePrefixAttribute_ThrowsInvalidOperationException()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "MissingAttribute", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).ForController<MissingAttributeController>().ForAction(x => x.Index()).SetRoutePrefix("de")
				.AddTranslation("Start", "de");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException),
			"No Route found for given Controller 'Home' and Action 'Index'.")]
		public void AddTranslation_MissingRoute_ThrowsInvalidOperationException()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection();

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Start", "de", "Home", "Index");
		}

		[TestMethod]
		[ExpectedExceptionWithMessage(typeof(InvalidOperationException), "AcceptedCultures does not contain culture 'de'.")]
		public void AddTranslation_NotAcceptedCulture_ThrowsInvalidOperationException()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Start", "de", "Home", "Index");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddTranslation_NullAction_ThrowsArgumentNullException()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection();

			// Act
			routeCollection.Localization(Configuration).ForController("Home").ForAction(null).AddTranslation("Start", "de");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddTranslation_NullController_ThrowsArgumentNullException()
		{
			// Arrange
			RouteCollection routeCollection = new RouteCollection();

			// Act
			routeCollection.Localization(Configuration).ForController(null).ForAction("Index").AddTranslation("Start", "de");
		}

		[TestMethod]
		public void AddTranslation_PrefixesExists_AddsPrefixesInCorrectOrderToTranslatedRoutes()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			Configuration.AddCultureAsRoutePrefix = true;

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Welcome", "Welcome", new { controller = "Home", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).SetAreaPrefix("Area").SetRoutePrefix("Route").AddTranslation("Willkommen", "de", "Home", "Index");

			// Assert
			Assert.IsTrue(routeCollection.Count == 2);

			Assert.IsTrue(((TranslationRoute)routeCollection[0]).Url == "en/Welcome");
			Assert.IsTrue(((TranslationRoute)routeCollection[1]).Url == "de/Area/Route/Willkommen");
		}

		[TestMethod]
		public void AddTranslation_ThreeRoutesExist_AppliesDefaultCultureToRootRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);
			routeCollection.MapRoute("Product", "Product/{category}/{id}", new { controller = "Home", action = "Product" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/{page}", "de", "Home", "Book");

			// Assert
			Assert.IsTrue(((TranslationRoute)routeCollection[1]).Culture == Configuration.DefaultCulture);
		}

		[TestMethod]
		public void AddTranslation_ThreeRoutesExist_AppliesSpecifiedCultureToTranslatedRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);
			routeCollection.MapRoute("Product", "Product/{category}/{id}", new { controller = "Home", action = "Product" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/{page}", "de", "Home", "Book");

			// Assert
			Assert.IsTrue(((TranslationRoute)routeCollection[2]).Culture == "de");
		}

		[TestMethod]
		public void AddTranslation_ThreeRoutesExist_ReplacesTranslatedRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);
			routeCollection.MapRoute("Product", "Product/{category}/{id}", new { controller = "Home", action = "Product" }, null);

			RouteBase route1 = routeCollection[0];
			RouteBase routeToTranslate = routeCollection[1];
			RouteBase route3 = routeCollection[2];

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/{page}", "de", "Home", "Book");

			// Assert
			Assert.IsTrue(routeCollection.Count == 4);
			Assert.AreSame(route1, routeCollection[0]);
			Assert.AreNotSame(routeToTranslate, routeCollection[1]);
			Assert.IsInstanceOfType(routeCollection[1], typeof(TranslationRoute));
			Assert.IsTrue(((TranslationRoute)routeCollection[1]).Url == "Book/{chapter}/{page}");
			Assert.IsInstanceOfType(routeCollection[2], typeof(TranslationRoute));
			Assert.IsTrue(((TranslationRoute)routeCollection[2]).Url == "Buch/{chapter}/{page}");
			Assert.AreSame(route3, routeCollection[3]);
		}

		[TestMethod]
		public void AddTranslation_ThreeRoutesExist_RootRouteContainsTranslatedRoute()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, null);
			routeCollection.MapRoute("Book", "Book/{chapter}/{page}", new { controller = "Home", action = "Book" }, null);
			routeCollection.MapRoute("Product", "Product/{category}/{id}", new { controller = "Home", action = "Product" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Buch/{chapter}/{page}", "de", "Home", "Book");

			// Assert
			Assert.IsTrue(
				((TranslationRoute)routeCollection[1]).TranslatedRoutes.Values.Contains((TranslationRoute)routeCollection[2]));
		}

		[TestMethod]
		public void AddTranslation_TwoRoutesForTheSameActionExists_ReplacesRoutesCorrect()
		{
			// Arrange
			Configuration.AcceptedCultures.Add("de");

			RouteCollection routeCollection = new RouteCollection();
			routeCollection.MapRoute("Home1", "Home1", new { controller = "Home", action = "Index" }, null);
			routeCollection.MapRoute("Home2", "Home2", new { controller = "Home", action = "Index" }, null);

			// Act
			routeCollection.Localization(Configuration).AddTranslation("Start1", "de", "Home", "Index");

			TranslationRoute translationRouteRootFirst = (TranslationRoute)routeCollection[0];
			TranslationRoute translationRouteFirst = (TranslationRoute)routeCollection[1];

			routeCollection.Localization(Configuration).AddTranslation("Start2", "de", "Home", "Index");

			// Assert
			Assert.IsTrue(routeCollection.Count == 4);

			Assert.AreSame(translationRouteRootFirst, routeCollection[0]);
			Assert.AreSame(translationRouteFirst, routeCollection[1]);
			Assert.IsInstanceOfType(routeCollection[2], typeof(TranslationRoute));
			Assert.IsTrue(((TranslationRoute)routeCollection[2]).Url == "Home2");
			Assert.IsInstanceOfType(routeCollection[3], typeof(TranslationRoute));
			Assert.IsTrue(((TranslationRoute)routeCollection[3]).Url == "Start2");
		}

		[TestMethod]
		public void ForAndSetFunctions_DefaultRouteTranslator_SavesValues()
		{
			// Arrange
			string controllerName = "Home";
			string controllerNamespace = "Namespace";
			string actionName = "Index";
			string areaPrefix = "AreaPrefix";
			string routePrefix = "RoutePrefix";
			string culture = "de";

			// Act
			RouteTranslator routeTranslator =
				new RouteTranslator().ForController(controllerName, controllerNamespace).ForAction(actionName).SetAreaPrefix(
					areaPrefix).SetRoutePrefix(routePrefix).ForCulture(culture);

			// Assert
			Assert.IsTrue(routeTranslator.Controller == controllerName);
			Assert.IsTrue(routeTranslator.ControllerNamespace == controllerNamespace);
			Assert.IsTrue(routeTranslator.Action == actionName);
			Assert.IsTrue(routeTranslator.AreaPrefix == areaPrefix);
			Assert.IsTrue(routeTranslator.RoutePrefix == routePrefix);
			Assert.IsTrue(routeTranslator.Culture == culture);
		}

		[TestMethod]
		public void StronglyTypedForAndSetFunctions_DefaultRouteTranslator_SavesValues()
		{
			// Arrange
			string controllerName = "MissingAttribute";
			string controllerNamespace = "RouteLocalizationMVC.Tests.Core";
			string actionName = "Index";
			string areaPrefix = "AreaPrefix";
			string routePrefix = "RoutePrefix";
			string culture = "de";

			// Act
			RouteTranslator<MissingAttributeController> routeTranslator =
				new RouteTranslator().ForController<MissingAttributeController>().ForAction(x => x.Index()).SetAreaPrefix(areaPrefix)
					.SetRoutePrefix(routePrefix).ForCulture(culture);

			// Assert
			Assert.IsTrue(routeTranslator.Controller == controllerName);
			Assert.IsTrue(routeTranslator.ControllerNamespace == controllerNamespace);
			Assert.IsTrue(routeTranslator.Action == actionName);
			Assert.IsTrue(routeTranslator.AreaPrefix == areaPrefix);
			Assert.IsTrue(routeTranslator.RoutePrefix == routePrefix);
			Assert.IsTrue(routeTranslator.Culture == culture);
		}

		[TestInitialize]
		public void TestInitialize()
		{
			Configuration = new Configuration
			{
				DefaultCulture = "en",
				ApplyDefaultCultureToRootRoute = true,
				AddCultureAsRoutePrefix = false,
				AcceptedCultures = new HashSet<string>() {"en"},
				ValidateURL = true,
				ValidateRouteArea = true,
				ValidateRoutePrefix = true,
				ValidateCulture = true
			};
		}
	}
}