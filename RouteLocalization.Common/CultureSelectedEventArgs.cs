#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;

	public class CultureSelectedEventArgs : EventArgs
	{
		public string SelectedCulture { get; set; }
	}
}