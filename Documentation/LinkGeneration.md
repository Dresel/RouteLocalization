# Link Generation

This section describes what you have to keep in mind when generating links with RouteLocalization.

## Basics

You can use the already existing functions in ASP.NET when generating links for RouteLocalization routes. When a link is generated from a RouteLocalization route it trys to find the specific translation for the current thread culture. When no localized route for this culture exists, it falls back to the neutral route.

For example consider this attribute route definition:

    [Route("Index")]
    public string Index()

You might generate a url like this:

    Url.Action("Index")

Suppose *DefaultCulture* is set to "en", *AddCultureAsRoutePrefix* is set to true and *AttributeRouteProcessing* is set to AddAsNeutralAndDefaultCultureRoute. With *TranslateInitialAttributeRoutes* this would result in following routes:

> /Index (neutral route)
> /en/Index (en route)

Suppose the thread culture is set to "de", RouteLocalization would first try to find a route for action "Index" and culture "de". Second it would look for a neutral route for action "Index". In this case "/Index" would be returned since there is no german route for Index.

Keep this in mind when generating links.

## The Culture Parameter

If you do want to generate a translated route for a specific language, you can also add the culture to the route values:

    Url.Action("Index", new { culture = "de" });

This would be useful for "switch language" links. If you want to generate a link to the current route but a different language, just do something like this within a view:

    RouteValueDictionary routeValueDictionaryEN = new RouteValueDictionary(ViewContext.RouteData.Values);
    routeValueDictionaryEN["culture"] = "en";

    <a href="@Url.RouteUrl(routeValueDictionaryEN)">To English Version of this site</a>

    RouteValueDictionary routeValueDictionaryDE = new RouteValueDictionary(ViewContext.RouteData.Values);
    routeValueDictionaryDE["culture"] = "de";

    <a href="@Url.RouteUrl(routeValueDictionaryDE)">To German Version of this site</a>

## Redirecting when user visits page first

A common usecase is to make the root url of a website as entry page and redirect the user to the localized version.

    [HttpGet]
    public RedirectResult Start()
    {
        // Redirect to localized Index
        return RedirectToAction("Index", new { culture = Thread.CurrentThread.CurrentCulture.Name });
    }

    [Route("Welcome")]
    [HttpGet]
    public string Index()
    {
        return View();
    }

Start can be mapped as classical route:

    routes.MapRoute("Start", string.Empty, new { controller = "Home", action = "Start" });

You could also add an neutral route instead:

    [Route]
    [HttpGet]
    public RedirectResult Start()
    {
        // Redirect to localized Index
        return RedirectToAction("Index", new { culture = Thread.CurrentThread.CurrentCulture.Name });
    }

    localization.ForController<HomeController>()
        .ForAction(x => x.Start())
        .AddNeutralTranslation();

Suppose we support two cultures "en" and "de" we would set *AttributeRouteProcessing* to AddAsDefaultCultureRoute and add a translation for "de":

    localization.ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
        .AddTranslation("Willkommen");

In combination with CultureSensitiveHttpModule / MessageHandler and DetectCultureFromBrowserUserLanguages (see the Getting Started section) the following would happen when someone browses to our site:

* If the user has set "en" as preferred language in the browser settings he would be redirect to "/Welcome".
* If the user has set "de" as preferred language in the browser settings he would be redirected to "/Willkommen".
* If the user has no language that matches "en" or "de" he would be redirected to "/Welcome" since "en" is our DefaultCulture.

[Back to ReadMe](../README.md)