# Selecting and Translating Routes

This section describes the various options for selecting and translating routes.

## The localization object

For translations you normally start with the *Localization* object. There are two possibilities:

1. Adding the route directly

If you do not want to use the fluent interfaces, you can call this three methods directly:

    AddTranslation(string url, string culture, string controller, string action, string controllerNamespace, ICollection<Type> actionArguments)

    AddTranslationForNamedRoute(string url, string culture, string namedRoute)

    AddTranslation(string url, string culture, LocalizationCollectionRoute route)

The parameters are used to find the correct route stored in the route collection. You can search by parameters, named routes or pass the localization route directly (an option you would only need in certain use cases). The Parameters *controllerNamespace* and *actionArguments* need only be set when there are multiple routes that would otherwise fit to the given controller and action name - for example two controllers with the same name in different namespaces or two overloaded actions which can only be distinguished by their action parameters.

2. Using fluent interfaces

The more elegant solution for selecting routes would be via fluent interfaces. For each parameter there is a corresponding *For* function:

    localization.ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
        .AddTranslation("Willkommen");

Except for the strictly typed version for controller and action this would be the same as

    localization.AddTranslation("Willkommen", "de", "Home", "Index", null, null);

You can also chain multiple translations:

    localization.ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
        .AddTranslation("Willkommen")
        .ForAction(x => x.Book())
        .AddTranslation("Buch/{chapter}/{page}");

### Translating RoutePrefix and RouteArea

Asp.Net attribute routing allows to use [RouteArea] and [RoutePrefix] attributes for controllers. You can set / translate those via the

    .SetAreaPrefix("AreaPrefix")
    
    .SetRoutePrefix("RoutePrefix")
    
methods. Taken from the sample project the controller could look something like this:

	[RoutePrefix("HomeWithRoutePrefixAttribute")]
	public partial class HomeWithRoutePrefixAttributeController : Controller
	{
	    ... actions
	}

And the corresponding translation:

    localization.ForCulture("de")
        .ForController<HomeWithRoutePrefixAttributeController>()
        .SetRoutePrefix("RoutePrefixDE")
        .ForAction(x => x.Index())
        .AddTranslation("Willkommen")

You don't have to translate RouteArea / RoutePrefix if you don't want to. If you leave out *SetAreaPrefix* / *SetRoutePrefix* the prefix from the attribute will be taken for url generation.

Note that Web API only supports the [RoutePrefix] attribute, so their will be no *SetAreaPrefix* function.

### Adding controller level translations

Asp.Net [Route] attributes can also be applied on controller level. To translate you only have to specifiy the controller without any action:

The Controller might look something like this:

    [Route("Welcome/{action}")]
    public class HomeController : Controller
    {
        ... actions

Configuration:

    localization.ForCulture("de")
        .ForController<HomeController>()
        .AddTranslation("Willkommen/{action}");

### Translating routes with constraints and defaults

Asp.Net attribute routing allows route constraints and defaults for parameters within the url template. Those are parsed and cut out when the route is actually created:

    [Route("Welcome/{action=index}")]

This route itself only stores "Welcome/{action}" within the Url property.

By default RouteLocalization runs some checks when urls are translated. If you get an exception similiar to this

> *Translation Route 'Willkommen/{action=index}' contains different { } placeholders than original Route 'Welcome/{action}'*

you specified url constraints for the translation too - which you shouldn't. Translate those routes like this

    localization.ForCulture("de")
        .ForController<HomeController>()
        .AddTranslation("Willkommen/{action}");

### Distinguish overloaded actions by parameters

If you have actions with the same name you must specifiy those so RouteLocalizations knows which route to translate:

    [Route("MyActionWithStringParameter/{parameter}")]
    ActionResult MyAction(string parameter)

    [Route("MyActionWithIntParameter/{parameter}")]
    ActionResult MyAction(int parameter)

you can specify those parameters within ForAction:

    .ForAction(x => x.MyAction(), new [] { typeof(int) })

    .ForAction(x => x.MyAction(), new [] { typeof(string) })

You could also consider using named routes for special cases like this:

    [Route("MyActionWithStringParameter/{parameter}", Name = "NamedRoute1")]
    ActionResult MyAction(string parameter)

    .ForNamedRoute("NamedRoute1")

### Translate similiar routes at once

You might have GET / POST actions defined like this:

    [HttpGet]
    [Route("Welcome")]
    public string Index()

    [HttpPost]
    [Route("Welcome")]
    public string Index(ViewModel value)

Those urls can be translated at once if you set *Configuration.AddTranslationToSimiliarUrls* to true. Otherwise you would have to call AddTranslation twice.

## Culture is automatically set as route default

If you need the culture for the current route within an action you can just add culture as parameter:

    [Route("Welcome")]
    public ActionResult Index(string culture)
    {
        // Culture is automatically set as default (route.Defaults) for localized routes,
        // it can also be requested via the route data values
        string cultureFromRouteData = (string)RouteData.Values["culture"];

        return View();
    }

## Where are the translations stored

Each attribute route is wrapped by the *LocalizationDirectRouteProvider* by a *LocalizationCollectionRoute* object. This special route object stores all translations for one specific attribute route. The *LocalizationCollectionRoute* routes are stored within the *Configuration.LocalizationCollectionRoutes* property.

[Back to ReadMe](../README.md)
