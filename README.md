# Nuget package

There is a nuget package avaliable here http://nuget.org/packages/RouteLocalizationMVC.

## Introduction

RouteLocalizationMVC is an MVC package that allows localization of your routes:

    routes
        .ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
            .AddTranslation("Willkommen")
        .ForAction(x => x.Book())
            .AddTranslation("Buch/{chapter}/{page}");

Before MVC5 I used AttributeRouting and its builtin localization whichs worked great - in MVC5 I prefer the built in MapMvcAttributeRoutes function in combination with the little RouteLocalizationMVC functionality for route translating.

RouteLocalizationMVC has the following behaviour:

  * At the beginning of every request, LocalizationHttpModule checks browser user languages, and sets the thread culture to this language if it is in Configuration.AcceptedCultures. If its not, thread culture is set to Configuration.DefaultCulture.
  * If a culture specific TranslationRoute is requested, the thread culture gets overridden by CultureSensitiveRouteHandler.
  * Every translated route is replaced by a TranslationRoute. The current culture is used for TranslationRoute.GetVirtualPath to force culture specific route generation. This is useful when UrlHelper.Route, RedirectToRoute or similiar functions are called.

For example, the neutral Index actions (entry point to website) redirects to localized routes. Once being on a localized route, every generated link is also localized for this culture.

    [AllowAnonymous]
    public partial class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public virtual ActionResult Index()
        {
            // This redirects to ~/Welcome or ~/Willkommen, depending on Browser Settings
            return RedirectToAction(MVC.Home.ActionNames.Welcome);
        }

        [HttpGet]
        [Route("Welcome")]
        public virtual ActionResult Welcome()
        {
            return View();
        }
    }

See WebSample for more information.