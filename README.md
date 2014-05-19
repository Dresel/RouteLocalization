# Nuget package

There is an alpha nuget package available here:

  * ASP.NET MVC: [RouteLocalization.Mvc](http://nuget.org/packages/RouteLocalization.Mvc)
  * ASP.NET Web API: [RouteLocalization.WebApi](http://nuget.org/packages/RouteLocalization.WebApi)

## Introduction

**RouteLocalization was rewritten for ASP.NET 5.2. The old versions (< 2.0) are not working for newer versions of ASP.NET (>= 5.1). This is because of the internal changes of attribute routing. Since 5.2 you can use attribute routing in combination with a *DirectRouteProvider*, which makes it possible to hook to the attribute routing process. This makes it possible to make RouteLocalization working again. There is also now an implementation for Web API available.**

RouteLocalization is a package that allows localization of your attribute routes with fluent interfaces:

    routes
        .ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
            .AddTranslation("Willkommen")
        .ForAction(x => x.Book())
            .AddTranslation("Buch/{chapter}/{page}");

It has similar functionality as the localization features of AttributeRouting but with MVC 5.2 it can be used as lightweight addition to the existing *MapMvcAttributeRoutes* / *MapHttpAttributeRoutes* functionality.

RouteLocalization has the following features:

  * The *LocalizationDirectRouteProvider* replaces attribute routes (which are plain *Route* / *HttpRoute* instances) by *LocalizationCollectionRoute* instances
  * Every translation is added to the corresponding *LocalizationCollectionRoute*
  * *LocalizationCollectionRoute* is thread culture sensitive. When a path is requested (*GetVirtualPath*) - e.g. when generating an url - it uses the translated route when available. This ensures that for example german routes are used when the thread culture is german.

Optional features:

  * You can use the existing *CultureSensitiveHttpModule* (ASP.NET MVC) / *CultureSensitiveMessageHandler* (ASP.NET Web API) if you want to preinitialize the current thread culture. There is an existing implementation (*DetectCultureFromBrowserUserLanguages*) which sets the thread culture depending on the browser language settings. You can also set your own implementation, to e.g. get the culture from a cookie.
  * You can also use the *CultureSensitiveActionFilterAttribute* (recommended). If a culture specific (translated) route is requested, the thread culture is set to the route culture.

## Configuration

### Required

See also the [MVC](RouteLocalization.Mvc.Sample/App_Start/RouteConfig.cs) and [Web API](RouteLocalization.Http.Sample/App_Start/WebApiConfig.cs) configuration of the sample project.

To make localization of routes possible, you have to use the *LocalizationDirectRouteProvider*.

ASP.NET MVC:

    // For less code preparation use the static provider stored in Localization class
    routes.MapMvcAttributeRoutes(Localization.LocalizationDirectRouteProvider);

ASP.NET Web API:

    // For less code preparation use the static provider stored in Localization class
    config.MapHttpAttributeRoutes(Localization.LocalizationDirectRouteProvider);

There are various options which can be configured via the *Configuration* class. Since *MapHttpAttributeRoutes* isn't executed immediately you have to also call *ContinueAfterPreviousInitialization*.

ASP.NET MVC:

    Localization localization = routes.Localization(configuration =>
    {
        configuration.DefaultCulture = "en";
        configuration.AcceptedCultures = new HashSet<string>() { "en", "de" };

        ... other settings
    }

ASP.NET Web API:

    config.ContinueAfterPreviousInitialization(httpConfiguration =>
    {
        Localization localization = routes.Localization(configuration =>
        {
            configuration.DefaultCulture = "en";
            configuration.AcceptedCultures = new HashSet<string>() { "en", "de" };

            ... other settings
        }
    }

You can now use the *Localization* object. If you don't set *AttributeRouteProcessing* to *None* you have to first call *TranslateInitialAttributeRoutes*. Afterwards you can define your translations.

ASP.NET MVC & Web API:

    localization.TranslateInitialAttributeRoutes();

    localization.Translate(localization =>
    {
        localization.ForCulture("de")
            .ForController<HomeController>()
            .ForAction(x => x.Index())
            .AddTranslation("Willkommen");

         ... additional translations
    });

### Optional

You can optionally configure and setup the *CultureSensitiveHttpModule* / *CultureSensitiveMessageHandler* - you can also define your own *GetCultureFromHttpContextDelegate* / *GetCultureFromHttpRequestMessageDelegate* delegate.

ASP.NET MVC:

    CultureSensitiveHttpModule.GetCultureFromHttpContextDelegate =
        Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture);

ASP.NET Web API:

    config.MessageHandlers.Add(new CultureSensitiveMessageHandler()
    {
        GetCultureFromHttpRequestMessageDelegate =
            Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture)
    });

You can (and should) also setup the *CultureSensitiveActionFilterAttribute*:

ASP.NET MVC:

    ICollection<LocalizationCollectionRoute> localizationCollectionRoutes =
        Localization.LocalizationDirectRouteProvider.LocalizationCollectionRoutes.Select(
            x => (LocalizationCollectionRoute)x.Route).ToList();

    config.Filters.Add(new CultureSensitiveActionFilterAttribute(localizationCollectionRoutes));

ASP.NET Web API:

    ICollection<LocalizationCollectionRoute> localizationCollectionRoutes =
        Localization.LocalizationDirectRouteProvider.LocalizationCollectionRoutes.Select(
            x => (LocalizationCollectionRoute)x.Route).ToList();

    config.Filters.Add(new CultureSensitiveActionFilterAttribute(localizationCollectionRoutes));

## Miscellaneous

### Generate link for a specific culture

If you want to generate a translated route for a specific language, you can add the culture to the route values:

ASP.NET MVC:

    // Use action / controller names ...
    Url.Action("Index", new { culture = "de" });

    // ... or named routes
    Url.RouteUrl("Index", new { culture = "de" });

ASP.NET Web API:

    // Use named routes
    Url.Link("Index", new { culture = "de" })

### Configuration class

The *Configuration* class has a lot of parameters which can be modified, most of them are for validation purpose:

    // Set the default culture (used by various functions); set to "en" by default
    public string DefaultCulture

    // Set and define valid cultures (used by various functions); set to { "en" } by default
    HashSet<string> AcceptedCultures

    // Should the culture ("en", "de", ...) added as route prefix (e.g. "/en/Welcome");
    // set false by default
    bool AddCultureAsRoutePrefix 

    // Normally only the first route found is translated, if you want similiar routes,
    for example one GET Action and one POST Action with identical URL,
    translated at once, you can set this to true; set to false by default
    bool AddTranslationToSimiliarUrls

    // Defines how attribute routes should be processed
    // * None: There will be no routes except the ones you explicitly define in Translate()
    // * AddAsNeutralRoute: Every attribute route will be added as neutral route
    // * AddAsDefaultCultureRoute: Every attribute route will be added as localized route for defined default culture
    // * AddAsNeutralAndDefaultCultureRoute: Every attribute route will be added as neutral route and
    //   as localized route for defined default culture
    // * AddAsNeutralRouteAndReplaceByFirstTranslation: Every attribute route will be added as neutral route first, but when
    //   you add a translation for a route, the neutral route will be removed
    AttributeRouteProcessing AttributeRouteProcessing

    // Set to Localization.LocalizationDirectRouteProvider.LocalizationCollectionRoutes by default
    List<RouteEntry> LocalizationCollectionRoutes 

    // Other validation settings, set to true by default

    bool ValidateCulture 

    bool ValidateRouteArea

    bool ValidateRoutePrefix

    bool ValidateUrl
