# Getting Started

This section describes the minimal setup to get RouteLocalization up and running.

## 0. Prerequisites

RouteLocalization is a lightweight extension for Asp.Net Mvc / Web Api attribute routing. It depends on the DirectRouteProvider mechanism which was introduced with 5.2 - lower versions of Asp.Net Mvc / Web Api are NOT supported.

Routes that are not created via attribute routing (e.g. via MapRoute) cannot be localized.

## 1. Download

Use NuGet from Visual Studio to install the RouteLocalization package. RouteLocalization supports both ASP.NET MVC and Web Api:

For ASP.NET MVC use the following package:

> Install-Package RouteLocalization.Mvc

For ASP.NET Web API use the following package:

> Install-Package RouteLocalization.WebApi

You can use both version in one project, but you have to be careful not to mix up namespaces.

## 2. Setup LocalizationDirectRouteProvider

To get RouteLocalization to work you have to pass the LocalizationDirectRouteProvider when calling *MapMvcAttributeRoutes* or *MapHttpAttributeRoutes*. LocalizationDirectRouteProvider intercepts attribute routing and proxies generated routes with its own routes to make localization possible. You can use the static instance or create your own:

ASP.NET MVC:

    // For less code preparation use the static provider stored in Localization class
    routes.MapMvcAttributeRoutes(Localization.LocalizationDirectRouteProvider);

ASP.NET Web API:

    // For less code preparation use the static provider stored in Localization class
    config.MapHttpAttributeRoutes(Localization.LocalizationDirectRouteProvider);

## 3. Configure RouteLocalization

There are various options which can be configured via RouteLocalization's *Configuration* class. This class can be passed to the *Localization* instance:

    Configuration configuration = new Configuration()
    {
        DefaultCulture = "en",
        AcceptedCultures = new HashSet<string>() { "en", "de" }

        ... other settings
    };

    Localization localization = new Localization(configuration);

## 4. Translate routes with the localization object

First you have to decide how the initial attribute routes, which are intercepted by RouteLocalization, should be processed. There are a few possibilities that can be choosen via the *Configuration.AttributeRouteProcessing* property.

If your attribute routes for example are English be default, you could define that every attribute route should be added as localized route for English culture. You would therefore set the *DefaultCulture* to "en" and *AttributeRouteProcessing* to *AddAsDefaultCultureRoute*.

The initial AttributeRoutes are created when *TranslateInitialAttributeRoutes* gets called:

    localization.TranslateInitialAttributeRoutes();

Afterwards you can define your translations for other cultures:

    localization.ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
        .AddTranslation("Willkommen");

This adds a localized German route for the Index action of the Home controller with "Willkommen" as Url.

## 5. Using the extension methods

The previous code lines can be more fluently programmed as:

ASP.NET MVC:

    routes.Localization(configuration =>
    {
        // Configuration

        configuration.DefaultCulture = "en";
        configuration.AcceptedCultures = new HashSet<string>() { "en", "de" };

        configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;

    }).Translate(localization =>
    {
        // Translations

        localization.TranslateInitialAttributeRoutes();

        localization.ForCulture("de")
            .ForController<HomeController>()
            .ForAction(x => x.Index())
            .AddTranslation("Willkommen");
    });

Because *MapHttpAttributeRoutes* is not executed immediately, you have to call *ContinueAfterPreviousInitialization* for Web API first:

ASP.NET Web API:

    // Continue with localization configuration after Web API Routes configuration
    config.ContinueAfterPreviousInitialization(httpConfiguration =>
    {
        httpConfiguration.Routes.Localization(configuration =>
        {
            // Configuration

            configuration.DefaultCulture = "en";
            configuration.AcceptedCultures = new HashSet<string>() { "en", "de" };

            configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsDefaultCultureRoute;
        }).Translate(localization =>
        {
            // Translations

            localization.TranslateInitialAttributeRoutes();

            localization.ForCulture("de")
                .ForController<HomeController>()
                .ForAction(x => x.Index())
                .AddTranslation("Willkommen");
        });
    });

## 6. Add *CultureSensitiveActionFilterAttribute* (optional)

If you want the *Thread.CurrentCulture* and *Thread.CurrentUICulture* to be set to the culture of the current route, you can use the CultureSensitiveActionFilterAttribute:

ASP.NET MVC:

    GlobalFilters.Filters.Add(new CultureSensitiveActionFilterAttribute());

ASP.NET Web API:

    config.Filters.Add(new CultureSensitiveActionFilterAttribute());

So when some browses to "/Welcome" the thread culture will be set to English, when some browses to "/Willkommen" it will be set to German, etc.

## 7. Using the CultureSensitiveHttpModule / MessageHandler (optional)

If you want to preinitialize the *Thread.CurrentCulture* and *Thread.CurrentUICulture* depending on different parameters (for example the browser user languages), you can use the *CultureSensitiveHttpModule* / *CultureSensitiveMessageHandler*.

You can pass a delegate or use the already existing *DetectCultureFromBrowserUserLanguages* one.

ASP.NET MVC:

    CultureSensitiveHttpModule.GetCultureFromHttpContextDelegate =
        Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture);

ASP.NET Web API:

    config.MessageHandlers.Add(new CultureSensitiveMessageHandler()
    {
        GetCultureFromHttpRequestMessageDelegate =
            Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture)
    });

This could for example be used to set the preferred language of the user to one of the accepted cultures so you could redirect him to a localized route afterwards.

The HttpModule / MessageHandler gets called before the *CultureSensitiveActionFilterAttribute* is executed.

[Back to ReadMe](../README.md)