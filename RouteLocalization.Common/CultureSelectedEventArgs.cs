#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;

#if ASPNETWEBAPI
	using TContext = System.Web.Http.Controllers.HttpRequestContext;
#else
	using TContext = System.Web.HttpContextBase;
#endif

	public class CultureSelectedEventArgs : EventArgs
	{
		public TContext Context { get; set; }

		public string SelectedCulture { get; set; }
	}
}