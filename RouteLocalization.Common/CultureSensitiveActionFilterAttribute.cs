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
	using System.Web.Http.Controllers;
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
		public CultureSensitiveActionFilterAttribute(ICollection<LocalizationCollectionRoute> localizationCollectionRoutes)
		{
			SetCurrentCulture = true;
			SetCurrentUICulture = true;

			LocalizationCollectionRoutes = localizationCollectionRoutes;
		}

		public CultureSensitiveActionFilterAttribute(bool setCurrentCulture, bool setCurrentUICulture,
			ICollection<LocalizationCollectionRoute> localizationCollectionRoutes)
		{
			SetCurrentCulture = setCurrentCulture;
			SetCurrentUICulture = setCurrentUICulture;

			LocalizationCollectionRoutes = localizationCollectionRoutes;
		}

		public event EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public ICollection<LocalizationCollectionRoute> LocalizationCollectionRoutes { get; set; }

		public bool SetCurrentCulture { get; set; }

		public bool SetCurrentUICulture { get; set; }

		public override void OnActionExecuting(TActionContext context)
		{
			LocalizationRoute route = LocalizationCollectionRoutes.Select(x =>
#if ASPNETWEBAPI
				x.GetLocalizationRoute(context.RequestContext.VirtualPathRoot, context.Request)).SingleOrDefault(x => x != null);
#else
				x.GetLocalizationRoute(context.HttpContext)).SingleOrDefault(x => x != null);
#endif

			// Route doesn't contain culture information so return
			if (route == null || string.IsNullOrEmpty(route.Culture))
			{
				return;
			}

			// Route contains culture information
			string cultureName = route.Culture;

			// Set culture
			CultureInfo cultureInfo = new CultureInfo(cultureName);

			if (SetCurrentCulture)
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}

			if (SetCurrentUICulture)
			{
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureName });
		}
	}
}