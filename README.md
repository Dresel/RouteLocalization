# Nuget package

There is a nuget package avaliable here http://nuget.org/packages/RouteLocalizationMVC.

## Introduction

RouteLocalizationMVC is an MVC package that allows localization of your routes with fluent interfaces:

    routes
        .ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
            .AddTranslation("Willkommen")
        .ForAction(x => x.Book())
            .AddTranslation("Buch/{chapter}/{page}");

It has similar functionality as the localization features of AttributeRouting but with MVC 5 it can be used as lightweight addition to the existing MapMvcAttributeRoutes functionality.

RouteLocalizationMVC has the following behaviour:

  * Preinitialisation of the current thread culture with the LocalizationHttpModule
  * If a culture specific route is requested, the thread culture gets overridden by CultureSensitiveRouteHandler
  * Every translated route is replaced by a TranslationRoute. The overridden GetVirtualPath uses the current culture for route generation. This ensures that for example german routes are used when the thread culture is german.

### Example: Use LocalizationHttpModule to initialize ThreadCulture

You can set the GetCultureFromHttpContextDelegate of the LocalizationHttpModule to initialize the thread culture as needed (cookie, domain tlc, etc.):

    // Set LocalizationHttpModule to initialize ThreadCulture from Browser UserLanguages
    LocalizationHttpModule.GetCultureFromHttpContextDelegate = httpContext =>
    {
        // Set default culture as fallback
        string cultureName = Configuration.DefaultCulture;
        
        if (httpContext.Request.UserLanguages != null)
        {
            // Get language from HTTP Header
            foreach (string userLanguage in httpContext.Request.UserLanguages.Select(x => x.Split(';').First()))
            {
                try
                {
                    CultureInfo userCultureInfo = new CultureInfo(userLanguage);
                    
                    // We don't can / want to support all languages
                    if (!Configuration.AcceptedCultures.Contains(userCultureInfo.Name.ToLower()))
                    {
                    	continue;
                    }
                
                    // Culture found that is supported
                    cultureName = userCultureInfo.Name.ToLower();
                    break;
                }
                catch
                {
                    // Ignore invalid cultures
                    continue;
                }
            }
        }
        
        // Return accepted culture
        return new CultureInfo(cultureName);
    };

### Example: Apply culture to every translated route

HomeController.cs

    public class HomeController : Controller
    {
        [Route("Welcome")]
        public ActionResult Index()
        {
            return View();
        }
    }

RouteConfig.cs

    // Apply culture to every translated route
    Configuration.AddCultureAsRoutePrefix = true;

    routes
        .ForCulture("de")
        .ForController<HomeController>()
        .ForAction(x => x.Index())
            .AddTranslation("Willkommen");

    // This results into one english generated route "~/en/Welcome" and one german generated route "~/de/Willkommen"

### Example: Force route generation for a specific language

If you want to generate a translated route for a specific language, you can add culture to RouteDictionary:

    @Url.RouteUrl(new RouteValueDictionary(ViewContext.RouteData.Values) { { "Culture", "en" } })

This would print the current route for english translation. This could be handy if you want to implement a "change culture" link.

### Configuration and Extension Hooks

The Configuration Class has a lot of static parameters that can be modified, most of them are for validation:

    public static HashSet<string> AcceptedCultures { get; set; }
    
    public static bool AddCultureAsRoutePrefix { get; set; }
    
    public static bool ApplyDefaultCultureToRootRoute { get; set; }
    
    public static string DefaultCulture { get; set; }
    
    public static bool ValidateCulture { get; set; }
    
    public static bool ValidateRouteArea { get; set; }
    
    public static bool ValidateRoutePrefix { get; set; }
    
    public static bool ValidateURL { get; set; }

For the culture configuration, you can hook to the CultureSelected EventHandler, if you want to store the informationen for selected cultures (e.g. store culture in a ProfileStoarge that will be used for database queries):

    LocalizationHttpModule.CultureSelected += ...

    CultureSensitiveRouteHandler.CultureSelected += ...

More Information can be found in the WebSample.