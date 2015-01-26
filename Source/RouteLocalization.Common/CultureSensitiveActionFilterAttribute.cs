#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;

#if ASPNETWEBAPI
	using System.Web.Http.Filters;
	using RouteLocalization.Http.Routing;
	using TActionContext = System.Web.Http.Controllers.HttpActionContext;
#else
	using System.Web.Mvc;
	using RouteLocalization.Mvc.Routing;
	using TActionContext = System.Web.Mvc.ActionExecutingContext;
#endif

	public class CultureSensitiveActionFilterAttribute : ActionFilterAttribute
	{
		public CultureSensitiveActionFilterAttribute()
		{
			SetCurrentCulture = true;
			SetCurrentUICulture = true;
			TryToPreserverBrowserRegionCulture = false;
			AcceptedCultures = new HashSet<string>();
		}

		public CultureSensitiveActionFilterAttribute(bool setCurrentCulture, bool setCurrentUICulture)
		{
			SetCurrentCulture = setCurrentCulture;
			SetCurrentUICulture = setCurrentUICulture;
			TryToPreserverBrowserRegionCulture = false;
			AcceptedCultures = new HashSet<string>();
		}

		public event EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public bool SetCurrentCulture { get; set; }

		public bool SetCurrentUICulture { get; set; }

		public bool TryToPreserverBrowserRegionCulture { get; set; }

		public ISet<string> AcceptedCultures { get; set; }

		public override void OnActionExecuting(TActionContext context)
		{
			LocalizationRoute route = context.RequestContext.RouteData.Route as LocalizationRoute;

			// Route doesn't contain culture information so return
			if (route == null || string.IsNullOrEmpty(route.Culture))
			{
				return;
			}

			// Route contains culture information
			string cultureName = route.Culture;

			// Set culture
			CultureInfo cultureInfo = new CultureInfo(cultureName);

			// Try to override to region dependent browser culture
			if (TryToPreserverBrowserRegionCulture)
			{
				if (AcceptedCultures.Count == 0)
				{
					throw new InvalidOperationException("TryToPreserverBrowserRegionCulture can only be used in combination with AcceptedCultures.");
				}

#if ASPNETWEBAPI
				cultureInfo =
					Localization.DetectCultureFromBrowserUserLanguages(
						new HashSet<string>(AcceptedCultures.Where(culture => culture.StartsWith(cultureInfo.Name))), cultureInfo.Name)(
							context.Request);
#else
				cultureInfo =
					Localization.DetectCultureFromBrowserUserLanguages(
						new HashSet<string>(AcceptedCultures.Where(culture => culture.StartsWith(cultureInfo.Name))), cultureInfo.Name)(
							context.HttpContext.ApplicationInstance.Context);
#endif
			}

			if (SetCurrentCulture)
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}

			if (SetCurrentUICulture)
			{
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}

#if ASPNETWEBAPI
			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureName, Context = context.RequestContext });
#else
			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureName, Context = context.HttpContext });
#endif
		}
	}
}