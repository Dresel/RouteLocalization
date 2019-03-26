namespace RouteLocalization.Mvc.Sample.Controllers
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LocalizedRouteAttribute : Attribute
    {
        public LocalizedRouteAttribute(string route, string culture)
        {
            Route = route;
            Culture = culture;
        }

        public string Culture { get; }

        public string Route { get; }
    }
}