namespace RouteLocalization.Mvc.Sample
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using RouteLocalization.Mvc.Extensions;
    using RouteLocalization.Mvc.Routing;
    using RouteLocalization.Mvc.Sample.Controllers;
    using RouteLocalization.Mvc.Setup;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // New workflow since 5.2
            // This provider wraps generated attribute routes with LocalizationCollectionRoute routes
            ////LocalizationDirectRouteProvider provider
            ////	= new LocalizationDirectRouteProvider(new DefaultDirectRouteProvider());

            // For less code preparation use the static provider stored in Localization class
            routes.MapMvcAttributeRoutes(Localization.LocalizationDirectRouteProvider);

            const string defaultCulture = "en";
            ISet<string> acceptedCultures = new HashSet<string>() { defaultCulture, "en-US", "de", "de-AT" };

            // Add translations
            // You can translate every specific route that contains default Controller and Action (which MapMvcAttributeRoutes does)
            routes.Localization(configuration =>
            {
                // Important: Set the route collection from LocalizationDirectRouteProvider if you specify your own
                //// configuration.LocalizationCollectionRoutes = provider.LocalizationCollectionRoutes;

                configuration.DefaultCulture = defaultCulture;
                configuration.AcceptedCultures = acceptedCultures;

                // Dont use build in initial processing
                configuration.AttributeRouteProcessing = AttributeRouteProcessing.None;
                configuration.AddCultureAsRoutePrefix = true;
                configuration.AddTranslationToSimiliarUrls = true;
            }).Translate(localization =>
            {
                // Foreach intercepted attribute route...
                localization.Configuration.LocalizationCollectionRoutes.ForEach(route =>
                {
                    // ... check if its an action level attribute route ...
                    if (((bool?)route.Route.DataTokens[RouteDataTokenKeys.TargetIsAction]) == true)
                    {
                        // Action Level AttributeRoute

                        // ... get ActionDescriptor for the underlying route object ...
                        var actionDescriptor = ((ActionDescriptor[])route.Route.DataTokens[RouteDataTokenKeys.Actions])
                            .Single();

                        localization.AddNeutralTranslation((LocalizationCollectionRoute)route.Route);

                        foreach (LocalizedRouteAttribute localizedRouteAttribute in actionDescriptor
                            .GetCustomAttributes(false).OfType<LocalizedRouteAttribute>())
                        {
                            localization.AddTranslation(localizedRouteAttribute.Route, localizedRouteAttribute.Culture,
                                (LocalizationCollectionRoute)route.Route);
                        }
                    }
                    else
                    {
                        // Controller Level AttributeRoute

                        // ... and add as default culture route if its an Controller Level AttributeRoute
                        localization.AddTranslation(route.Route.Url(), localization.Configuration.DefaultCulture,
                            (LocalizationCollectionRoute)route.Route);
                    }
                });
            });

            // Optional
            // Setup CultureSensitiveHttpModule
            // This Module sets the Thread Culture and UICulture from http context
            // Use predefined DetectCultureFromBrowserUserLanguages delegate or implement your own
            CultureSensitiveHttpModule.GetCultureFromHttpContextDelegate =
                Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture);

            // Optional
            // Add culture sensitive action filter attribute
            // This sets the Culture and UICulture when a localized route is executed
            GlobalFilters.Filters.Add(new CultureSensitiveActionFilterAttribute()
            {
                // Set this options only if you want to support detection of region dependent cultures
                // Supports this use case: https://github.com/Dresel/RouteLocalization/issues/38#issuecomment-70999613
                AcceptedCultures = acceptedCultures, TryToPreserverBrowserRegionCulture = true
            });
        }
    }
}