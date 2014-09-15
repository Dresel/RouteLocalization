#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;
	using System.Globalization;
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
		}

		public CultureSensitiveActionFilterAttribute(bool setCurrentCulture, bool setCurrentUICulture)
		{
			SetCurrentCulture = setCurrentCulture;
			SetCurrentUICulture = setCurrentUICulture;
		}

		public event EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public bool SetCurrentCulture { get; set; }

		public bool SetCurrentUICulture { get; set; }

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