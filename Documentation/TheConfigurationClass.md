# The Configuration Class

This section describes the various configuration options for RouteLocalization.

## AddCultureAsRoutePrefix

When *AddCultureAsRoutePrefix* is set to true, the culture is added as prefix to every localized route (e.g. "en/Welcome", "de/Willkommen", ...).

## AttributeRouteProcessing

This property controls how the initial attribute routes are processed when *TranslateInitialAttributeRoutes* is called. The different possibilities are best explained by looking at the following example:

Suppose we have the following two actions:

    [Route("Index")]
    public string Index()

    [Route("SomeOtherAction")]
    public string Index(object value)

Normally attribute routing would add two routes similar to "/Index" and "/SomeOtherAction". When using RouteLocalization this might be different, depending on AttributeRouteProcessing.

There are the following options:

* None
When TranslateInitialAttributeRoutes is called it simply does nothing. You would get the same result if you don't call this function at all. This would result in having no routes.

* AddAsNeutralRoute
This would add "/Index" and "/SomeOtherAction" as neutral route. Neutral routes are routes where culture == string.Empty. You might need neutral routes for routes the are culture independent (e.g. routes to images) or for routes that are not yet translated. Neutral routes are also used for link generation when no translated route for a specific culture exist.

* AddAsDefaultCultureRoute
This would add those routes as localized routes, depending on the *DefaultCulture*. If *AddCultureAsRoutePrefix* is set to true, the prefix would also be added. Suppose *DefaultCulture* is set to "en" and *AddCultureAsRoutePrefix* is set to true, this would result in having two localized routes "/en/Index" and "/en/SomeOtherAction".

* AddAsNeutralAndDefaultCultureRoute
This would add those routes as neutral and localized routes. Suppose *DefaultCulture* is set to "en" and *AddCultureAsRoutePrefix* is set to true, this would result in having four routes "/Index", "/SomeOtherAction", "/en/Index" and "/en/SomeOtherAction".

* AddAsNeutralRouteAndReplaceByFirstTranslation
This would add "/Index" and "/SomeOtherAction" as neutral route. Neutral routes will be removed when they are explicitly translated via *AddTranslation*.

    localization.ForCulture("en")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
        .AddTranslation("Start");

Suppose *AddCultureAsRoutePrefix* is set to true, this would remove the existing neutral route "/Index" and would replace it by "/en/Start".

## DefaultCulture and AcceptedCultures

The *DefaultCulture* is used by the *TranslateInitialAttributeRoutes* function when is set to *AddAsDefaultCultureRoute* or *AddAsNeutralAndDefaultCultureRoute*.

*AcceptedCultures* is used for validation when translating routes as long as *ValidateCulture* is not set to false.

## AddTranslationToSimiliarUrls

If *AddTranslationToSimiliarUrls* is set to true, AddTranslation would translate similar routes at once. Otherwise you would have to call AddTranslation twice.

This could be useful for routes like this:

    [HttpGet]
    [Route("Welcome")]
    public string Index()

    [HttpPost]
    [Route("Welcome")]
    public string Index(ViewModel value)

## ValidateCulture, ValidateRouteArea, ValidateRoutePrefix, ValidateUrl

There are some validations within RouteLocalization which can disabled via those properties. Only recommended if you know what you are doing.

[Back to ReadMe](../README.md)