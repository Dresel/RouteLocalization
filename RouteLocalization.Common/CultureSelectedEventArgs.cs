#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;

#if ASPNETWEBAPI
	using THttpObject = System.Net.Http.HttpRequestMessage;
#else
	using THttpObject = System.Web.HttpContext;
#endif

	public class CultureSelectedEventArgs : EventArgs
	{
		public string SelectedCulture { get; set; }

		public THttpObject HttpObject { get; set; }
	}
}